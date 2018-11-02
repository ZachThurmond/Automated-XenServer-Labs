    If (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {

    #Relaunch PowerShell as an elevated process in order to execute certain commands
    Start-Process powershell.exe ('{0}' -f $($MyInvocation.MyCommand.Path -replace ' ', '` ')) -Verb RunAs
    exit

    }
    
Cd ($MyInvocation.MyCommand.Path).Remove(($MyInvocation.MyCommand.Path).LastIndexOf("\"))

$HideConsole = '[DllImport("user32.dll")] public static extern bool ShowWindow(int handle, int state);'
add-type -name win -member $HideConsole -namespace native
[native.win]::ShowWindow(([System.Diagnostics.Process]::GetCurrentProcess() | Get-Process).MainWindowHandle, 0)

Add-Type -AssemblyName PresentationFramework
Add-Type -AssemblyName System.Windows.Forms

[System.Collections.ArrayList]$Global:OldIPAddresses = @()
[System.Collections.ArrayList]$Global:AllCreatedServers = @()
[System.Collections.ArrayList]$Global:DefaultGateways = @()
[System.Collections.ArrayList]$Global:SubnetMasks = @()
[System.Collections.ArrayList]$Global:IPAddresses = @()
[System.Collections.ArrayList]$Global:PrimaryDNSServers = @()
[System.Collections.ArrayList]$Global:SecondaryDNSServers = @()
[System.Collections.ArrayList]$Global:GetVMIPs = @()
[System.Collections.ArrayList]$Global:FinalForms = @()
[System.Collections.ArrayList]$Global:CAServers = @()
[System.Collections.ArrayList]$Global:CANames = @()
[System.Collections.ArrayList]$Global:CATypes = @()
[System.Collections.ArrayList]$Global:ParentCA = @()
[System.Collections.ArrayList]$Global:CAHashAlgorithm = @()
[System.Collections.ArrayList]$Global:CACryptoProvider = @()
[System.Collections.ArrayList]$Global:CAKeyLength = @()
[System.Collections.ArrayList]$Global:CAValidityPeriod = @()
[System.Collections.ArrayList]$Global:CAValidityPeriodUnits = @()
[System.Collections.ArrayList]$Global:CAResponder = @()
[System.Collections.ArrayList]$Global:CAWebEnrollment = @()
[System.Collections.ArrayList]$Global:OULocation = @()
[System.Collections.ArrayList]$Global:AllCreatedOUs = @()
[System.Collections.ArrayList]$Global:GroupName = @()
[System.Collections.ArrayList]$Global:GroupOULocation = @()
[System.Collections.ArrayList]$Global:GroupScope = @()
[System.Collections.ArrayList]$Global:GroupType = @()
[System.Collections.ArrayList]$Global:UserLoginName = @()
[System.Collections.ArrayList]$Global:UserFirstName = @()
[System.Collections.ArrayList]$Global:UserLastName = @()
[System.Collections.ArrayList]$Global:UserPassword = @()
[System.Collections.ArrayList]$Global:UserOULocation = @()
[System.Collections.ArrayList]$Global:DFSServers = @()
[System.Collections.ArrayList]$Global:DFSRoots = @()
[System.Collections.ArrayList]$Global:DFSFolders = @()
[System.Collections.ArrayList]$Global:DFSFolderRoot = @()
[System.Collections.ArrayList]$Global:DFSFolderTarget = @()

$Global:StopWatch = New-Object -TypeName System.Diagnostics.Stopwatch 
$Global:XenServerModule = (Get-ChildItem (Get-Location) -Recurse -ErrorAction SilentlyContinue | where {$_.Name -eq "XenServerPowerShell.dll"}).FullName 
$Global:ISOCreationFormRan = $False
$Global:CACounter = 0
$Global:PreviouslySelectedIndex = 0


#################################  GENERAL FUNCTIONS  #################################

Function CheckServerOnState([string] $CheckComputer) {

    while(!(Test-Connection -ComputerName $CheckComputer -Count 1 -ErrorAction SilentlyContinue)) {

    WaitScript 1
    [System.Windows.Forms.Application]::DoEvents()

    }

}


Function WaitJob($Job) {

    While ($Job.State -eq 'Running') {

    Start-Sleep -Milliseconds 500
    [System.Windows.Forms.Application]::DoEvents()

    }

}


Function WaitScript([int] $Time) {

$WaitStopWatch = New-Object -TypeName System.Diagnostics.Stopwatch 

$WaitStopWatch.Start()

    While($WaitStopWatch.Elapsed.Seconds -ne $Time){

    Start-Sleep -Milliseconds 500
    [System.Windows.Forms.Application]::DoEvents()

    }

$WaitStopWatch.Stop()
$WaitStopWatch.Reset()

}


#################################  XENSERVER  #################################

Function ImportModule {

    if($Global:XenServerModule) {
    
    Import-Module $Global:XenServerModule

    }

    else {

    [System.Windows.MessageBox]::Show("`t`t`t       ERROR`nThe XenServer module that is required for proper functionality was not found in any subdirectories where the script was run from, please input the path to the module.")
    XenServerModuleForm

    }

}


Function ValidateModulePath {

$ModuleValidationCount = 0
    
    if(Test-Path $ModuleLocationTextBox.Text){

        if($ModuleLocationTextBox.Text -notmatch [regex]'[a-zA-Z][:][\\]{1}[a-zA-Z0-9\-_.$\\]{1,}' -or $ModuleLocationTextBox.Text -notmatch [regex]'\\\\(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)[a-zA-Z0-9\-_$\\]{1,}' -and $ModuleLocationTextBox.Text -notmatch "XenServerPowerShell.dll") {
                
        [System.Windows.MessageBox]::Show("Error in Module Path TextBox: The full path to XenServerPowerShell.dll must be included as well as XenServerPowerShell.dll itself")
        $ModuleContinueButton.Enabled = $False
                
        }else{ $ModuleValidationCount++ }

    }
    else{
    
    [System.Windows.MessageBox]::Show("Error in Module Path TextBox: The path provided does not exist")

    }

    if($ModuleValidationCount -eq 1){
    
    $ModuleContinueButton.Enabled = $True

    $Global:XenServerModule = $ModuleLocationTextBox.Text

    Import-Module $Global:XenServerModule
    
    }
    
}


Function ConnectToXenHost {

    Try {
    
    $Global:Session = Connect-XenServer -Url "https://$($XenHostTextBox.Text)" -UserName $UsernameTextBox.Text -Password $PasswordTextBox.Text -NoWarnCertificates -SetDefaultSession

    } 
  
    Catch [XenAPI.Failure] {
       
    [System.Windows.MessageBox]::Show("$($_.Exception), try connecting to $($_.Exception.ErrorDescription[1])")

    }

}


Function DisconnectXenHost {

    if($Global:Session) {

    $Global:Session | Disconnect-XenServer

    } 

}


Function GetSR {

$StorageNameLabels = (Get-XenSR -ErrorAction SilentlyContinue | Sort -Property name_label | select type,name_label | where { $_.type -ne "iso" -and $_.name_label -ne "Removable storage" -and $_.name_label -ne "DVD drives" -and $_.name_label -ne "XenServer Tools" -and $_.type -ne "iso" }).name_label

    if($DropDownStorage.Items) {

    $DropDownStorage.Items.Clear()

    }

    foreach($StorageLabel in $StorageNameLabels) {

    $DropDownStorage.Items.Add($StorageLabel) | Out-Null

    }

}


Function GetAllTemplates {

    If($DefaultTemplateCheckbox.Checked -eq $True) {

    $AllDefaultXenTemplates = (Get-XenVM -ErrorAction SilentlyContinue | Where { $_.is_a_template -eq $True -and $_.other_config.default_template } | sort -Property name_label).name_label
        
        if($DropDownTemplates.Items) {
        
        $DropDownTemplates.Items.Clear()
        
        }
       
        foreach($DefaultXenTeplate in $AllDefaultXenTemplates) {     

        $DropDownTemplates.Items.Add($DefaultXenTeplate) | Out-Null

        }

    }
    
    else {
    
    $AllCustomXenTemplates = (Get-XenVM -ErrorAction SilentlyContinue | Where { $_.is_a_template -eq $True -and !($_.other_config.default_template) -and $_.is_a_snapshot -eq $False} | sort -Property name_label).name_label
        
        if($DropDownTemplates.Items) {
        
        $DropDownTemplates.Items.Clear()
        
        }

        foreach($CustomXenTeplate in $AllCustomXenTemplates) {

        $DropDownTemplates.Items.Add($CustomXenTeplate) | Out-Null

        }

    }

}


Function GetRAM {
$RAMNumber = 1
$MaxRAM = (Get-XenHostMetrics -ErrorAction SilentlyContinue).memory_total | % {("{0:N0}" -f ($_ / 1GB)-1)} | sort | select -First 1

    if($DropDownRAMAmmount.Items) {

    $DropDownRAMAmmount.Items.Clear()

    }

    while($RAMNumber -le ($MaxRAM/$NewVMHostnameListBox.Items.Count)) {

    $DropDownRAMAmmount.Items.Add($RAMNumber)
    $RAMNumber++

    }
}


Function GetDiskSize {
$DiskSize = 20
$MaxDiskSize = ((Get-XenSR -Name $DropDownStorage.SelectedItem).physical_size - (Get-XenSR -Name $DropDownStorage.SelectedItem).physical_utilisation) / 1GB 

    If($DropDownDiskSize.Items) {

    $DropDownDiskSize.Items.Clear()

    }

    while($DiskSize -le ($MaxDiskSize/$NewVMHostnameListBox.Items.Count)) {

    $DropDownDiskSize.Items.Add($DiskSize)
    $DiskSize++

    }
}


Function GetCPUCount {

$BaseCPUCount = 1
$MaxCPUCount = ((Get-XenHostCpu -ErrorAction SilentlyContinue).number | sort -Descending | select -Unique -First 1)+1

    if($DropDownCPUCount.Items) {

    $DropDownCPUCount.Items.Clear()

    }

    while($BaseCPUCount -le $MaxCPUCount) {

    $DropDownCPUCount.Items.Add($BaseCPUCount)
    $BaseCPUCount++

    }

}


Function GetISOs {

    if($DropDownISORepository.SelectedItem) {

    $AllISOs = (Get-XenSR -Name $DropDownISORepository.SelectedItem | select -ExpandProperty VDIs | Get-XenVDI | where {$_.name_label -match ".iso"}).name_label | Sort

        if($DropDownISOs.Items) {

        $DropDownISOs.Items.Clear()

        }

        foreach($ISOFile in $AllISOs) {
    
        $DropDownISOs.Items.Add($ISOFile) | Out-Null

        }

    }
    
}


Function GetISOLibrary {

$ISORepository = (Get-XenSR | where {$_.type -eq "iso" -and $_.name_label -ne "XenServer Tools"}).name_label
$ISOPath = (Get-XenSR | where {$_.type -eq "iso" -and $_.name_label -ne "XenServer Tools"}).name_description.Split("[")[1].Replace("]","")
$DropDownISORepository.Items.Clear()

    foreach($Repository in $ISORepository) {

    $DropDownISORepository.Items.Add($Repository) | Out-Null

    }

    if(($DropDownISORepository.Items).count -eq 1) {

    $DropDownISORepository.SelectedItem = $DropDownISORepository.Items[0]
    $DropDownISORepository.Enabled = $False
    
    }

    else {
    
    $DropDownISORepository.Enabled = $True
    
    }


}


Function GetServerNetworks {

$AllNetworks = (Get-XenNetwork -ErrorAction SilentlyContinue).name_label | Sort

    if($DropDownNetwork.Items) {

    $DropDownNetwork.Items.Clear()

    }

    foreach($XenNetwork in $AllNetworks) {

    $DropDownNetwork.Items.Add($XenNetwork)
       
    }

}


Function BuildVMs {

# Specify DropDown variables
$VMNames = $NewVMHostnameListBox.Items
$SourceTemplateName = $DropDownTemplates.SelectedItem
$StorageRepositoryName = $DropDownStorage.SelectedItem
$SelectedNetwork = $DropDownNetwork.SelectedItem
 
$VMCreationProgressTextBox.AppendText("====================")
$VMCreationProgressTextBox.AppendText("`r`nStarting Creation of VMs")
$VMCreationProgressTextBox.AppendText("`r`n====================")

    foreach($VMName in $VMNames){

    # Specify general properties 
    $GetSRProperties = Get-XenSR -Name $StorageRepositoryName
    $GetNetworkProperties = Get-XenNetwork $SelectedNetwork
    $TemplateSRLocation = (Get-XenVM -Name $SourceTemplateName | Select -ExpandProperty VBDs | Get-XenVBD | Select -ExpandProperty VDI | Get-XenVDI | Select -ExpandProperty SR | Get-XenSR).name_label
    $ObjSourceTemplate = Get-XenVM -Name $SourceTemplateName

        if($DefaultTemplateCheckbox.CheckState -eq "Checked") { 

        # Specify required VM properties
        $VMRAM = ($DropDownRAMAmmount.SelectedItem*1GB)
        $DiskSize = ($DropDownDiskSize.SelectedItem*1GB)
        $VMCPU = $DropDownCPUCount.SelectedItem
        
        # Create new VM from all specified properties
        $VMCreationProgressTextBox.AppendText("`r`nCreating VM - $VMName")
        New-XenVM -NameLabel $VMName -MemoryTarget $VMRAM -MemoryStaticMax $VMRAM -MemoryDynamicMax $VMRAM -MemoryDynamicMin $VMRAM -MemoryStaticMin $VMRAM -VCPUsMax $VMCPU -VCPUsAtStartup $VMCPU -HVMBootPolicy "BIOS order" -HVMBootParams @{ order = "dc" } -HVMShadowMultiplier 1 -UserVersion 1 -ActionsAfterReboot restart -ActionsAfterCrash restart -ReferenceLabel $ObjSourceTemplate.reference_label -HardwarePlatformVersion 2 -Platform @{ "cores-per-socket" = "$VMCPU"; hpet = "true"; pae = "true"; vga = "std"; nx = "true"; viridian_time_ref_count = "true"; apic = "true"; viridian_reference_tsc = "true"; viridian = "true"; acpi = "1" } -OtherConfig @{ base_template_name = $ObjSourceTemplate.reference_label }
        
        $GetVMProperties = Get-XenVM -Name $VMname
        
        WaitScript 1

        # Create a new Virtual Disk with the same name as the new VM
        $VMCreationProgressTextBox.AppendText("`r`nCreating Hard Disk for $VMName")
        New-XenVDI -NameLabel $VMName -VirtualSize $DiskSize -SR $GetSRProperties -Type user

        WaitScript 4

        # Specify VDI and Network locations
        $NewVDI = Get-XenVDI -Name $VMName
        $VIFDevice = (Get-XenVMProperty -VM $GetVMProperties -XenProperty AllowedVIFDevices)[0]

            if($GetVMProperties -and $NewVDI){
                
                # Create CD drive for the new VM
                $VMCreationProgressTextBox.AppendText("`r`nCreating CD Drive for $VMName")
                New-XenVBD -VM $GetVMProperties -VDI $null -Type CD -mode RO -Userdevice 3 -Bootable $False -Unpluggable $True -Empty $True
                
                # Attach previously created hard drive into the new VM
                $VMCreationProgressTextBox.AppendText("`r`nAttaching Hard Disk to $VMName")
                New-XenVBD -VM $GetVMProperties -VDI $NewVDI -Type Disk -mode RW -Userdevice 0 -Bootable $True -Unpluggable $True

                # Create network interface for the new VM
                $VMCreationProgressTextBox.AppendText("`r`nCreating Network Card for $VMName")
                New-XenVIF -VM $GetVMProperties -Network $GetNetworkProperties -Device $VIFDevice 
                
                # Mount previously created hard disk
                $VMCreationProgressTextBox.AppendText("`r`nMounting Hard Disk for $VMName")
                Get-XenVM -Name $VMName | Select -ExpandProperty VBDs | Get-XenVBD | where {$_.type -eq "Disk"} | Select -ExpandProperty VDI | Set-XenVDI -NameLabel $VMName  
                
            }
            
        }
 
        if($DefaultTemplateCheckbox.CheckState -eq "Unchecked") {

            if($TemplateSRLocation -match $GetSRProperties.name_label) {

            # Create a clone of the template
            $VMCreationProgressTextBox.AppendText("`r`nCreating VM - $VMName")
            Invoke-XenVM -NewName $VMName -VM $ObjSourceTemplate -XenAction Clone

            # Provision the copy into a VM
            Invoke-XenVM -XenAction Provision -Name $VMName

            WaitScript 1

            # Rename the attached disk to the name of the VM
            $VMCreationProgressTextBox.AppendText("`r`nRenaming Virtual Disk Attached to $VMName")
            Get-XenVM -Name $VMName | Select -ExpandProperty VBDs | Get-XenVBD | where {$_.type -eq "Disk"} | Select -ExpandProperty VDI | Set-XenVDI -NameLabel $VMName

            }

            else {
            
            # Copy the chosen template to the SR where the VMs are being created 
            $VMCreationProgressTextBox.AppendText("`r`nCreating Temporary Template From $SourceTemplateName")         
            Invoke-XenVM -NewName "$SourceTemplateName - TEMP" -VM $ObjSourceTemplate  -SR $GetSRProperties -XenAction Copy

            # Specify old and new template names
            $SourceTemplateName = "$SourceTemplateName - TEMP"
            $ObjSourceTemplate = Get-XenVM -Name $SourceTemplateName
            
            # Clone the template that was just coppied to create the first VM
            $VMCreationProgressTextBox.AppendText("`r`nCreating VM - $VMName")
            Invoke-XenVM -NewName $VMName -VM $ObjSourceTemplate -XenAction Clone
            
            # Provision the copy into a VM
            Invoke-XenVM -XenAction Provision -Name $VMName

            WaitScript 1

            # Rename the attached disk to the name of the VM
            $VMCreationProgressTextBox.AppendText("`r`nRenaming Virtual Disk Attached to $VMName")
            Get-XenVM -Name $VMName | Select -ExpandProperty VBDs | Get-XenVBD | where {$_.type -eq "Disk"} | Select -ExpandProperty VDI | Set-XenVDI -NameLabel $VMName

            # Rename the temporary templates attached disk name
            $ObjSourceTemplate | Select -ExpandProperty VBDs | Get-XenVBD | where {$_.type -eq "Disk"} | Select -ExpandProperty VDI | Set-XenVDI -NameLabel $SourceTemplateName

            }
        
        }

        if($BlankTemplateCheckbox.CheckState -eq 'Checked' -and $DropDownISOs.SelectedItem) {

        $SelectedBootISO = $DropDownISOs.Text

        # Get the VM, select the CD drive for the VM and attach the ISO
        $VMCreationProgressTextBox.AppendText("`r`nInserting $SelectedBootISO into $VMName")
        Get-XenVM -Name $VMName | Select -ExpandProperty VBDs | Get-XenVBD | where {$_.type -eq "CD"} | Invoke-XenVBD -XenAction Insert -VDI (Get-XenVDI -Name $SelectedBootISO).opaque_ref

        }

    # Start the created VM to begin installing the attached ISO
    $VMCreationProgressTextBox.AppendText("`r`nStarting VM - $VMName")
    $VM = Get-XenVM -Name $VMName
    Invoke-XenVM -VM $VM -XenAction Start -Async

    $VMCreationProgressTextBox.AppendText("`r`n=============================================")

    $Global:AllCreatedServers += $VMName

    }

    #If a temporary template was created, remove it and the associated disk
    if($SourceTemplateName -match "- TEMP") {
    
    WaitScript 1
    
    $ObjSourceTemplate | Select -ExpandProperty VBDs | Get-XenVBD | where {$_.type -eq "Disk"} | Select -ExpandProperty VDI | Remove-XenVDI

    WaitScript 1

    $VMCreationProgressTextBox.AppendText("`r`nRemoving Temporary Template $SourceTemplateName")
    Remove-XenVM -Name $SourceTemplateName

    }

$Global:StartTime = (Get-Date)
$Global:StopWatch.Start()

}


Function CheckVMFormProperties {

$VMValidationCount = 0

    if($DropDownStorage.SelectedItem -eq $null) {
    
    [System.Windows.MessageBox]::Show("Error in Storage Location DropDown: No storage location selected")
    $CreateNewVMButton.Enabled = $False

    }else{$VMValidationCount++}


    if(!$NewVMHostnameListBox.Items) {
                
    [System.Windows.MessageBox]::Show("Error in VM ListBox: There are no VMs listed")
    $CreateNewVMButton.Enabled = $False
                
    }else{$VMValidationCount++}
    

    if($DropDownTemplates.SelectedItem -eq $null) {
    
    [System.Windows.MessageBox]::Show("Error in Templates DropDown: No template selected")
    $CreateNewVMButton.Enabled = $False

    }else{$VMValidationCount++}


    if($DropDownISORepository.SelectedItem -eq $null) {
    
    [System.Windows.MessageBox]::Show("Error in ISO Repository DropDown: No ISO repository selected")
    $CreateNewVMButton.Enabled = $False

    }else{$VMValidationCount++}

    
    if($BlankTemplateCheckbox.CheckState -eq "Checked"){

        if($DropDownISOs.SelectedItem -eq $null) {
    
        [System.Windows.MessageBox]::Show("Error in ISO Selection DropDown: No ISO selected")
        $CreateNewVMButton.Enabled = $False

        }else{$VMValidationCount++}

    }


    if($DefaultTemplateCheckbox.CheckState -eq "Checked"){

        if($DropDownRAMAmmount.SelectedItem -eq $null) {
    
        [System.Windows.MessageBox]::Show("Error in RAM DropDown: No RAM size selected")
        $CreateNewVMButton.Enabled = $False

        }else{$VMValidationCount++}


        if($DropDownCPUCount.SelectedItem -eq $null) {
    
        [System.Windows.MessageBox]::Show("Error in CPU DropDown: No CPU count selected")
        $CreateNewVMButton.Enabled = $False

        }else{$VMValidationCount++}


        if($DropDownDiskSize.SelectedItem -eq $null) {
    
        [System.Windows.MessageBox]::Show("Error in Disk Size DropDown: No disk size selected")
        $CreateNewVMButton.Enabled = $False

        }else{$VMValidationCount++}


        if($DropDownNetwork.SelectedItem -eq $null) {
    
        [System.Windows.MessageBox]::Show("Error in Network DropDown: No network selected")
        $CreateNewVMButton.Enabled = $False

        }else{$VMValidationCount++}

    }


    If($BlankTemplateCheckbox.CheckState -eq 'Unchecked' -and $VMValidationCount -eq 4) {
    
    $CreateNewVMButton.Enabled = $True

    }


    elseif($BlankTemplateCheckbox.CheckState -eq 'Checked' -and $VMValidationCount -eq 5) {
    
    $CreateNewVMButton.Enabled = $True

    }


    elseif($BlankTemplateCheckbox.CheckState -eq 'Unchecked' -and $DefaultTemplateCheckbox.CheckState -eq "Checked" -and $VMValidationCount -eq 8) {
    
    $CreateNewVMButton.Enabled = $True

    }


    elseif($BlankTemplateCheckbox.CheckState -eq 'Checked' -and $DefaultTemplateCheckbox.CheckState -eq "Checked" -and $VMValidationCount -eq 9) {
    
    $CreateNewVMButton.Enabled = $True

    }


    else {
    
    $CreateNewVMButton.Enabled = $False
    
    }

}


#################################  ISO CREATION  #################################

Function CopyFiles {

# Specify the Selected ISO and the mount location
$SelectedISO = $ISOPathTextBox.Text
$MountedImage = Mount-DiskImage $SelectedISO -PassThru
$MountLocation = "$(($MountedImage | Get-Volume).DriveLetter):\"

$CopyRunspace = [runspacefactory]::CreateRunspace()
$CopyPowerShell = [System.Management.Automation.PowerShell]::Create()
$CopyPowerShell.runspace = $CopyRunspace
$CopyRunspace.Open()
$ISOCopyProgressLabel.Visible = $True

    $CopyParameterList = @{

        TargetFolder = $TargetFolderTextBox.Text
        SelectedISO = $ISOPathTextBox.Text
        MountLocation = "$(($MountedImage | Get-Volume).DriveLetter):\"
        MountedFiles = Get-ChildItem $MountLocation -Recurse
        CopyFileProgressBar = $CopyFileProgressBar
        ISOCopyProgressLabel = $ISOCopyProgressLabel
        MountImageFileCount = (Get-ChildItem $MountLocation -Recurse).Count
        TargetFolderFileCount = (Get-ChildItem $TargetFolder -Recurse).count
        AutounattendXML = $AutounattendPathTextBox.Text
        AdminPW = $AdminPasswordTextBox.Text
        AdminAcct = $AdminNameTextBox.Text
        ProductKey = $ProductKeyTextBox.Text
        XenToolsPath = $XenToolsPathTextBox.Text
        Server2016CheckBox = $Server2016CheckBox.CheckState
        Windows10Checkbox = $Windows10Checkbox.CheckState
        DropDownEditionSelection = $DropDownEditionSelection.SelectedItem
        DropDownTimeZone = $DropDownTimeZones.SelectedItem

    }

[void]$CopyPowerShell.AddScript({
 
Param ($TargetFolder, $SelectedISO, $MountedImage, $MountLocation, $MountedFiles, $CopyFileProgressBar, $ISOCopyProgressLabel, $MountImageFileCount, $TargetFolderFileCount, $AutounattendXML, $AdminPW, $AdminAcct, $ProductKey, $XenToolsPath, $Server2016CheckBox, $Windows10Checkbox, $DropDownEditionSelection, $DropDownTimeZones)

    [pscustomobject]@{

    TargetFolder = $TargetFolder
    SelectedISO = $SelectedISO
    MountedImage = $MountedImage
    MountLocation = $MountLocation
    MountedFiles = $MountedFiles
    CopyFileProgressBar = $CopyFileProgressBar
    ISOCopyProgressLabel = $ISOCopyProgressLabel
    MountImageFileCount = $MountImageFileCount
    TargetFolderFileCount = $TargetFolderFileCount
    AutounattendXML = $AutounattendXML
    AdminPW = $AdminPW
    AdminAcct = $AdminAcct
    ProductKey = $ProductKey
    XenToolsPath = $XenToolsPath
    Server2016CheckBox = $Server2016CheckBox
    Windows10Checkbox = $Windows10Checkbox
    DropDownEditionSelection = $DropDownEditionSelection
    DropDownTimeZones = $DropDownTimeZones

    }

    $CopyFileProgressBar.Maximum = $MountImageFileCount
    $ISOCopyProgressLabel.Text = "Copying files from $MountLocation to $TargetFolder"

    if($MountLocation) {

        Foreach($MountedFile in $MountedFiles) {

            # Copy all contents from the mounted ISO to the target location keeping the same file/folder structure 
            if ($MountedFile.PSIsContainer) {

            Copy-Item $MountedFile.FullName -Destination (Join-Path $TargetFolder $MountedFile.Parent.FullName.Substring($MountLocation.length))
        
            } 

            else {
                
            Copy-Item $MountedFile.FullName -Destination (Join-Path $TargetFolder $MountedFile.FullName.Substring($MountLocation.length)) 

            }

            $CopyFileProgressBar.PerformStep()

        }
        
    }

    # Copy Autounattend.xml file into the root of the ISO
    if($AutounattendXML -and $TargetFolder) {

    Copy-Item $AutounattendXML -Destination $TargetFolder"\Autounattend.xml"

    # Specify which Windows version is being installed
    if($Server2016CheckBox -eq 'Checked') {

    $WindowsEdition = "Windows Server 2016 $DropDownEditionSelection"

    }

    elseif($Windows10Checkbox -eq 'Checked') {
    
    $WindowsEdition = "Windows 10 $DropDownEditionSelection"
    
    }

    # Define the contents of the Autounattend.xml file for later modification
    $DefaultXML = Get-Content $TargetFolder"\Autounattend.xml"

        $DefaultXML | Foreach-Object {

            # Replace the contents of the Autounattend.xml file with the information provided
            $_ -replace '1AdminAccount', $AdminAcct `
            -replace '1AdminPassword', $AdminPW `
            -replace '1ProductKey', $ProductKey `
            -replace '1XenToolsPath', $XenToolsPath.Substring($XenToolsPath.LastIndexOf("\")+1) `
            -replace '1Edition', $WindowsEdition `
            -replace '1TimeZone', $DropDownTimeZones

        } | Set-Content $TargetFolder"\Autounattend.xml"

    }

    # If it was specified to install XenServer Tools, copy the parent folder into the target
    if($XenToolsPath) {
    
    Copy-Item $XenToolsPath -Destination $TargetFolder -Recurse
    
    }


}).AddParameters($CopyParameterList)

$CopyPowerShell.Invoke()

$MountImageFileCount = (Get-ChildItem $MountLocation -Recurse).Count
$TargetFolderFileCount = (Get-ChildItem $TargetFolder -Recurse).count

    if($MountedImage) {

    Dismount-DiskImage $SelectedISO

    }

}


Function BuildISO {

$SelectedISO = $ISOPathTextBox.Text
$TargetFolder = "`"$($TargetFolderTextBox.Text)`""
$NewISOName = $NewISONameTextBox.Text
$BootFile = "`"$($BootFilePathTextBox.Text)`""
$ISOTool = $ISOToolPathTextBox.Text

# List of arguments to pass to oscdimg.exe
$ArgumentList = "-b$BootFile -u2 -h -m $TargetFolder `"$($SelectedISO.Remove($SelectedISO.LastIndexOf("\")))\$NewISOName.iso`""

# Display in the form what ISO is being created and where
$ISOCopyProgressLabel.Text = "Creating $NewISOName.iso at $($SelectedISO.Remove($SelectedISO.LastIndexOf("\")))\"

# Create Custom ISO file. This turns the folder that contains the ISO and unattend into a new ISO file
Start-Process -FilePath $ISOTool -ArgumentList $ArgumentList -WindowStyle Hidden -Wait

$Global:ISOCreationFormRan = $True

}


Function BrowseFiles {

$OpenFileDialog = New-Object System.Windows.Forms.OpenFileDialog
$OpenFileDialog.initialDirectory = Get-Location
$OpenFileDialog.filter = "All files (*.*)| *.*"
$OpenFileDialog.ShowDialog() | Out-Null
$OpenFileDialog.filename

}


Function BrowseFolders {

$OpenFolderDialog = New-Object System.Windows.Forms.FolderBrowserDialog
$OpenFolderDialog.ShowDialog() | Out-Null
$OpenFolderDialog.SelectedPath

}


Function AddTimeZones {

$TimeZones = (Get-TimeZone -ListAvailable).StandardName | Sort

    foreach($TimeZone in $TimeZones) {

    $DropDownTimeZones.Items.Add($TimeZone)

    }

}


Function AddEditions {

$Windows10Editions = @("Home","Pro","Enterprise")
$Server2016Editions = @("SERVERSTANDARD", "SERVERDATACENTERCORE")

    if($Windows10Checkbox.CheckState -eq 'Checked') {

        foreach($Windows10Edition in $Windows10Editions){
        
        $DropDownEditionSelection.Items.Add($Windows10Edition)

        }

    }

    if($Server2016Checkbox.CheckState -eq 'Checked') {

        foreach($Server2016Edition in $Server2016Editions){
        
        $DropDownEditionSelection.Items.Add($Server2016Edition)

        }

    }

}


Function AutoPopulateTextBoxes {

$ScriptRunPath = Get-Location
$AllPathItems = Get-ChildItem $ScriptRunPath -Recurse | select DirectoryName, Name | where {$_.Name -notmatch ".cab" -and $_.Name -notmatch ".cs" -and $_.Name -notmatch ".cat" -and $_.Name -notmatch ".mui" -and $_.Name -notmatch ".ttf"}
$SearchEtfsboot = $AllPathItems | where {$_.Name -match "etfsboot.com"}
$SearchOscdimg = $AllPathItems | where {$_.Name -match "oscdimg.exe"}
$SearchXenTools = $AllPathItems | where {$_.Name -match "managementagentx64.msi"}

    if($SearchEtfsboot -and $SearchEtfsboot.Name.Count -eq 1 -and $BootFilePathTextBox.Text.Length -eq 0) {
    
    $BootFilePathTextBox.Text = "$($SearchEtfsboot.DirectoryName)\$($SearchEtfsboot.Name)"
    
    }   

    if($SearchOscdimg -and $SearchOscdimg.Name.Count -eq 1 -and $ISOToolPathTextBox.Text.Length -eq 0) {
    
    $ISOToolPathTextBox.Text = "$($SearchOscdimg.DirectoryName)\$($SearchOscdimg.Name)"
    
    }
    
    if($SearchXenTools -and $SearchXenTools.Name.Count -eq 1 -and $InstallXenToolsCheckbox.CheckState -eq 'Checked' -and $SearchXenTools.Text.Length -eq 0){
    
    $XenToolsPathTextBox.Text = $SearchXenTools.DirectoryName
    
    }
    
}


Function CheckISOCompletionState {

$ISOValidationCount = 0
    
    if($ISOPathTextBox.Text.Length -ge 1) {

        if(Test-Path $ISOPathTextBox.Text -ErrorAction SilentlyContinue) {

            if($ISOPathTextBox.Text -notmatch [regex]'[a-zA-Z][:][\\]{1}[a-zA-Z0-9\-_.$\\]{1,}' -or $ISOPathTextBox.Text -notmatch [regex]'\\\\(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)[a-zA-Z0-9\-_$\\]{1,}' -and $ISOPathTextBox.Text -notmatch ".iso") {
                
            [System.Windows.MessageBox]::Show("Error in ISO Path TextBox: The full path to the ISO must be included as well as the ISO itself")
            $CreateISOButton.Enabled = $False
                
            }else{ $ISOValidationCount++ }

        }

        else {
    
        [System.Windows.MessageBox]::Show("Error in ISO Path TextBox: The path provided does not exist")

        }

    }

    else {
    
    [System.Windows.MessageBox]::Show("Error in ISO Path TextBox: No path provided")
    
    }


    if($TargetFolderTextBox.Text.Length -ge 1) {

        if(Test-Path $TargetFolderTextBox.Text -ErrorAction SilentlyContinue) {

            if($TargetFolderTextBox.Text -notmatch [regex]'[a-zA-Z][:][\\][a-zA-Z0-9\-_$\\]{1,}' -or $TargetFolderTextBox.Text -notmatch [regex]'\\\\([a-zA-Z0-9\.]{1,3}){4}[a-zA-Z0-9\-_$\\]{1,}' -and $TargetFolderTextBox.Text -notmatch ".") {
    
            [System.Windows.MessageBox]::Show("Error in Target Folder TextBox: Only include the path to a folder")
            $CreateISOButton.Enabled = $False

            }else{ $ISOValidationCount++ }

        }

        else {
    
        [System.Windows.MessageBox]::Show("Error in Target Folder TextBox: The path provided does not exist")

        }

    }

    else {
    
    [System.Windows.MessageBox]::Show("Error in Target Folder TextBox: No path provided")

    }


    if($AutounattendPathTextBox.Text.Length -ge 1) {

        if(Test-Path $AutounattendPathTextBox.Text -ErrorAction SilentlyContinue) {

            if($AutounattendPathTextBox.Text -notmatch [regex]'[a-zA-Z][:][\\][a-zA-Z0-9\-_.$\\]{1,}' -or $AutounattendPathTextBox.Text -notmatch [regex]'\\\\([a-zA-Z0-9\.]{1,3}){4}[a-zA-Z0-9\-_$.\\]{1,}' -and $AutounattendPathTextBox.Text -notmatch "Autounattend.xml") {
    
            [System.Windows.MessageBox]::Show("Error in Autounattend.xml TextBox: Make sure to include Autounattend.xml as well as the full path to it")
            $CreateISOButton.Enabled = $False

            }else{ $ISOValidationCount++ } 

        }

        else {
    
        [System.Windows.MessageBox]::Show("Error in Autounattend.xml TextBox: The path provided does not exist")

        }

    }

    else {
    
    [System.Windows.MessageBox]::Show("Error in Autounattend.xml TextBox: No path provided")
    
    }


    if($BootFilePathTextBox.Text.Length -ge 1) {

        if(Test-Path $BootFilePathTextBox.Text -ErrorAction SilentlyContinue) {

            if($BootFilePathTextBox.Text -notmatch [regex]'[a-zA-Z][:][\\][a-zA-Z0-9\-_.$\\]{1,}' -or $BootFilePathTextBox.Text -notmatch [regex]'\\\\([a-zA-Z0-9\.]{1,3}){4}[a-zA-Z0-9\-_$.\\]{1,}' -and $BootFilePathTextBox.Text -notmatch "etfsboot.com") {
    
            [System.Windows.MessageBox]::Show("Error in Boot File TextBox: Make sure to include etfsboot.com as well as the full path to it")
            $CreateISOButton.Enabled = $False
        
            }else{ $ISOValidationCount++ }
    
        }

        else {
    
        [System.Windows.MessageBox]::Show("Error in Boot File TextBox: The path provided does not exist")

        }

    }

    else {
    
    [System.Windows.MessageBox]::Show("Error in Boot File TextBox: No path provided")
    
    }


    if($ISOToolPathTextBox.Text.Length -ge 1) {

        if(Test-Path $ISOToolPathTextBox.Text -ErrorAction SilentlyContinue) {

            if($ISOToolPathTextBox.Text -notmatch [regex]'[a-zA-Z][:][\\][a-zA-Z0-9\-_.$\\]{1,}' -or $ISOToolPathTextBox.Text -notmatch [regex]'\\\\([a-zA-Z0-9\.]{1,3}){4}[a-zA-Z0-9\-_$.\\]{1,}' -and $ISOToolPathTextBox.Text -notmatch "oscdimg.exe") {
    
            [System.Windows.MessageBox]::Show("Error in ISO Creation Tool TextBox: Make sure to include oscdimg.exe as well as the full path to it")
            $CreateISOButton.Enabled = $False

            }else{ $ISOValidationCount++ }
    
        }

        else {
    
        [System.Windows.MessageBox]::Show("Error in ISO Creation Tool TextBox: The path provided does not exist")

        }

    }

    else {
    
    [System.Windows.MessageBox]::Show("Error in ISO Creation Tool TextBox: No path provided")
    
    }


    If($InstallXenToolsCheckbox.CheckState -eq 'Checked') {

        if($XenToolsPathTextBox.Text.Length -ge 1) {

            if(Test-Path $XenToolsPathTextBox.Text) {

                if($XenToolsPathTextBox.Text -notmatch [regex]'[a-zA-Z][:][\\][a-zA-Z0-9\-_$\\]{1,}' -or $XenToolsPathTextBox.Text -notmatch [regex]'\\\\([a-zA-Z0-9\.]{1,3}){4}[a-zA-Z0-9\-_$\\]{1,}') {
    
                $ISOValidationCount++ 

                }
        
                else{ 
        
                [System.Windows.MessageBox]::Show("Error in Xen Tools TextBox: The Check box for installing XenServer Tools is enabled but no path was selected")
                $CreateISOButton.Enabled = $False 
        
                }

            }

            else {
    
            [System.Windows.MessageBox]::Show("Error in Xen Tools TextBox: The path provided does not exist")

            }

        }

        else {
        
        [System.Windows.MessageBox]::Show("Error in Xen Tools TextBox: No path provided")

        }
        
    }


    if($AdminNameTextBox.Text.Length -lt 1) {
    
    [System.Windows.MessageBox]::Show("Error in Admin Name TextBox: No admin name provided")
    $CreateISOButton.Enabled = $False

    }else{ $ISOValidationCount++ }


    if($AdminPasswordTextBox.Text.Length -lt 1) {
    
    [System.Windows.MessageBox]::Show("Error in Admin Password TextBox: No password provided")
    $CreateISOButton.Enabled = $False

    }else{ $ISOValidationCount++ }


    if($ProductKeyTextBox.Text -notmatch [regex]'([A-Za-z0-9\-]{6}){4}[A-Za-z0-9]{5}') {
    
    [System.Windows.MessageBox]::Show("Error in Product Key TextBox: The product key must be in XXXXX-XXXXX-XXXXX-XXXXX-XXXXX format")
    $CreateISOButton.Enabled = $False

    }else{ $ISOValidationCount++ }


    if($NewISONameTextBox.Text.Length -lt 1) {
    
    [System.Windows.MessageBox]::Show("Error in New ISO Name TextBox: Input a new ISO name")
    $CreateISOButton.Enabled = $False

    }else{ $ISOValidationCount++ }


    if($Windows10Checkbox.CheckState -ne 'Unchecked' -or $Server2016CheckBox.CheckState -ne 'Unchecked') {
    
        if($DropDownEditionSelection.SelectedItem -eq $null) {

        [System.Windows.MessageBox]::Show("Error in Edition Selection DropDown: No edition selected")
        $CreateISOButton.Enabled = $False

        }else{ $ISOValidationCount++ }

    }
    
    else {
    
    [System.Windows.MessageBox]::Show("Error in Edition Selection Checkboxes: No OS selected")
    
    }


    if($DropDownTimeZones.SelectedItem -eq $null) {

    [System.Windows.MessageBox]::Show("Error in TimeZone Selection DropDown: No TimeZone selected")
    $CreateISOButton.Enabled = $False

    }else{ $ISOValidationCount++ }

    
    if($InstallXenToolsCheckbox.CheckState -eq 'Unchecked' -and $ISOValidationCount -eq 11) {
    
    $CreateISOButton.Enabled = $True

    }


    Elseif($InstallXenToolsCheckbox.CheckState -eq 'Checked' -and $ISOValidationCount -eq 12) {
    
    $CreateISOButton.Enabled = $True

    }


    else {
    
    $CreateISOButton.Enabled = $False
    
    }

}


#################################  DOMAIN BUILDOUT  #################################

Function EnableMakeDCPrimary {

    if($DomainControllersListBox.Items.Count -ge 2) {
                
    $MakePrimaryButton.Enabled = $True
                
    }

    else {
                
    $MakePrimaryButton.Enabled = $False
                
    }

    if($DomainControllersListBox.Items.Count -eq 1 -and $DomainControllersListBox.Items[0] -notmatch [regex]"\*") {
    
    $DomainControllersListBox.Items.Add("$($DomainControllersListBox.Items[0])*")
    $DomainControllersListBox.Items.Remove($DomainControllersListBox.Items[0])
    
    }

}


Function AssociateVMsWithOldIPs {

    foreach($Server in ($Global:AllCreatedServers | sort)) {

    $ServerIPAddress = Get-XenVM -Name $Server | Select -ExpandProperty guest_metrics | Get-XenVMGuestMetrics

        if($ServerIPAddress.networks.ContainsKey("0/ipv4/0")) {

        $Global:OldIPAddresses += $ServerIPAddress.networks.'0/ipv4/0'

        }

    }

}


Function ChangeIPAddresses {

    foreach($XenVMServer in ($Global:AllCreatedServers | sort)) {

    #Define necessary parameters for IP configuration
    $ConnectionPassword = ConvertTo-SecureString -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $ConnectionCreds = New-Object -typename System.Management.Automation.PSCredential -ArgumentList "$($Global:OldIPAddresses[($Global:AllCreatedServers | sort).IndexOf($XenVMServer)])\$($LocalUsernameTextBox.Text)",$ConnectionPassword
    $NewIPAddress = $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($XenVMServer)]
    $PrefixLength = Convert-IpAddressToMaskLength $Global:SubnetMasks[($Global:AllCreatedServers | sort).IndexOf($XenVMServer)]
    $DefaultGateway = $Global:DefaultGateways[($Global:AllCreatedServers | sort).IndexOf($XenVMServer)]
    $DNSServers = "$($Global:PrimaryDNSServers[($Global:AllCreatedServers | sort).IndexOf($XenVMServer)]),$($Global:SecondaryDNSServers[($Global:AllCreatedServers | sort).IndexOf($XenVMServer)])"

    $VMStatusTextBox.AppendText("`r`nChanging the IP Address of $XenVMServer to $NewIPAddress")

        Invoke-Command -ComputerName $Global:OldIPAddresses[($Global:AllCreatedServers | sort).IndexOf($XenVMServer)] -credential $ConnectionCreds -ScriptBlock {

            param ($NewIPAddress, $PrefixLength, $DefaultGateway, $DNSServers)

            #Define the original IP address
            $OriginalIPAddress = ((Get-NetIPConfiguration).IPv4Address).IPAddress

            #Set the DNS Servers
            Set-DnsClientServerAddress -InterfaceAlias (Get-NetIPConfiguration).InterfaceAlias -ServerAddresses $DNSServers

            #Disable IPv6
            Disable-NetAdapterBinding -InterfaceAlias (Get-NetIPConfiguration).InterfaceAlias -ComponentID ms_tcpip6

            #Set the new IP address with the IP, Subnet Mask, and Default Gateway
            New-NetIPAddress -IPAddress $NewIPAddress -InterfaceAlias (Get-NetIPConfiguration).InterfaceAlias -PrefixLength $PrefixLength -DefaultGateway $DefaultGateway
                
                #Remove the old IP configuration only if the new and old IPs don't match
                if((((Get-NetIPConfiguration).IPv4Address).IPAddress | where {$_ -match $OriginalIPAddress}) -and ($NewIPAddress -NotMatch $OriginalIPAddress)) {

                Remove-NetIPAddress -IPAddress (((Get-NetIPConfiguration).IPv4Address).IPAddress | where {$_ -match $OriginalIPAddress}) -InterfaceAlias (Get-NetIPConfiguration).InterfaceAlias -Confirm:$False

                }

        } -ArgumentList $NewIPAddress, $PrefixLength, $DefaultGateway, $DNSServers -AsJob
    
    WaitScript 5

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function ChangeComputerNames {

    foreach($DCServer in $DomainControllersListBox.Items) {

    $ConnectionPassword = ConvertTo-SecureString -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $ConnectionCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer)])\$($LocalUsernameTextBox.Text)",$ConnectionPassword

        if($DCServer -match [regex]'\*'){
            
        $DCServer = $DCServer.Replace("*","")
            
        }

    $VMStatusTextBox.AppendText("`r`nChanging $($Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer)]) Machine Name to $DCServer")

        $RenameDC = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer)] -credential $ConnectionCreds -ScriptBlock {

        param ($DCServer)

        Rename-Computer -NewName $DCServer -Restart

        } -ArgumentList $DCServer -AsJob

        WaitJob $RenameDC

    }

    foreach($DomainServer in $AllServersListBox.Items) {

    $ConnectionPassword = ConvertTo-SecureString -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $ConnectionCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DomainServer)])\$($LocalUsernameTextBox.Text)",$ConnectionPassword

    $VMStatusTextBox.AppendText("`r`nChanging $($Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DomainServer)]) Machine Name to $DomainServer")

        $RenameMember = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DomainServer)] -credential $ConnectionCreds -ScriptBlock {

        param ($DomainServer)

        Rename-Computer -NewName $DomainServer -Restart

        } -ArgumentList $DomainServer -AsJob

        WaitJob $RenameMember

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function InstallDCComponents {

    foreach($DCServer in $DomainControllersListBox.Items) {

        if($DCServer -match [regex]'\*') {
            
        $DCServer = $DCServer.Replace("*","")
            
        }

    $ConnectionPassword = ConvertTo-SecureString -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $ConnectionCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer)])\$($LocalUsernameTextBox.Text)",$ConnectionPassword

    $VMStatusTextBox.AppendText("`r`nInstalling Domain Controller Components on $DCServer")

        $DCComponents = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer)] -credential $ConnectionCreds -ScriptBlock {

        Install-WindowsFeature AD-Domain-Services -IncludeManagementTools

        } -AsJob

        WaitJob $DCComponents

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function PromotePrimaryDomainController {

    foreach($DCServer in ($DomainControllersListBox.Items | where {$_ -match [regex]'\*'})) {

    #Define Domain specific parameters
    $DomainName = $DomainNameTextBox.Text
    $SafeModePassword = ConvertTo-SecureString $SafeModePasswordTextBox.Text -AsPlainText -force
    $ConnectionPassword = ConvertTo-SecureString -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $ConnectionCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer.Replace("*",''))])\$($LocalUsernameTextBox.Text)",$ConnectionPassword

        if($DFSCheckbox.CheckState -eq "Checked") {
    
            $VMStatusTextBox.AppendText("`r`nInstalling DFSR Components on $($DCServer.Replace("*"," ")) for DFS Buildout")

            $DFSComponents = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer.Replace("*",""))] -credential $ConnectionCreds -ScriptBlock {

            #Install DFSR components if DFS was selected during component selection, this is necessary for DFS buildout functionality
            Install-WindowsFeature FS-DFS-Replication -IncludeManagementTools

            } -AsJob

            WaitJob $DFSComponents
    
        }

    $VMStatusTextBox.AppendText("`r`nPromoting $($DCServer.Replace("*"," ")) to a Domain Controller")

        $DCPromotion = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer.Replace("*",""))] -credential $ConnectionCreds -ScriptBlock {

        param ($DomainName,$SafeModePassword)

        #Create the AD DS Forest with the paramaeters specified in the AD DS buildout form
        Install-ADDSForest -DomainName $DomainName -SafeModeAdministratorPassword $SafeModePassword -DomainNetBIOSName $DomainName.Remove($DomainName.IndexOf(".")).ToUpper() -SYSVOLPath "C:\Windows\SYSVOL" -LogPath "C:\Windows\NTDS" -DatabasePath "C:\Windows\NTDS" -InstallDNS -Force

        } -ArgumentList $DomainName,$SafeModePassword -AsJob

        WaitJob $DCPromotion

        #If the Domain Controller does not reboot automatically within 15 seconds, reboot the machine
        if(Test-Connection -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer.Replace("*",""))] -Count 1 -ErrorAction SilentlyContinue) {

        Invoke-XenVM -Name $DCServer -XenAction CleanReboot

        }

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function PromoteSecondaryDomainControllers {

$VMStatusTextBox.AppendText("`r`n")

    foreach($DCServer in ($DomainControllersListBox.Items | where {$_ -notmatch [regex]'\*'})) {

    $DomainName = $DomainNameTextBox.Text
    $SafeModePassword = ConvertTo-SecureString $SafeModePasswordTextBox.Text -AsPlainText -force
    $ConnectionPassword = ConvertTo-SecureString -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $ConnectionCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer)])\$($LocalUsernameTextBox.Text)",$ConnectionPassword
    $DomainAdminCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

    $VMStatusTextBox.AppendText("`r`nPromoting $DCServer to a Domain Controller")

        $SecondaryJob = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCServer)] -credential $ConnectionCreds -ScriptBlock {

        param ($DomainName,$SafeModePassword,$DomainAdminCreds)

        Install-ADDSDomainController -DomainName $DomainName -Credential $DomainAdminCreds -SafeModeAdministratorPassword $SafeModePassword -SYSVOLPath "C:\Windows\SYSVOL" -LogPath "C:\Windows\NTDS" -DatabasePath "C:\Windows\NTDS" -InstallDns -Force 

        } -ArgumentList $DomainName,$SafeModePassword,$DomainAdminCreds -AsJob

        WaitJob $SecondaryJob

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function JoinToDomain {

    WaitScript 30

    foreach($DomainMachine in $AllServersListBox.Items) {

    $DomainName = $DomainNameTextBox.Text
    $ConnectionPassword = ConvertTo-SecureString -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $ConnectionCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DomainMachine)])\$($LocalUsernameTextBox.Text)",$ConnectionPassword
    $DomainAdminCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

    $VMStatusTextBox.AppendText("`r`nJoining $DomainMachine to the $DomainName Domain")

        $JoinToDomain = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DomainMachine)] -credential $ConnectionCreds -ScriptBlock {

        param ($DomainName,$DomainAdminCreds)

        Add-Computer -DomainName $DomainName -Credential $DomainAdminCreds -Restart -Force

        } -ArgumentList $DomainName,$DomainAdminCreds -AsJob

        WaitJob $JoinToDomain

    WaitScript 3

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function PopulateUsernameAndPassword {

    if($Global:ISOCreationFormRan -eq $True){
    
    $LocalUsernameTextBox.Text = $AdminNameTextBox.Text
    $LocalPasswordTextBox.Text = $AdminPasswordTextBox.Text

    }

}


Function DefinePreviouslySelectedIndex {

    while($Global:IPAddresses.Count -lt $IPSchemaListBox.Items.Count) {
    
    $Global:DefaultGateways += $null
    $Global:SubnetMasks += $null
    $Global:IPAddresses += $null
    $Global:PrimaryDNSServers += $null
    $Global:SecondaryDNSServers += $null

    }

    $Global:PreviouslySelectedIndex = $IPSchemaListBox.SelectedIndex

}


Function UpdateTextBoxes {

$DefaultGatewayTextBox.Enabled = $True
$SubnetMaskTextBox.Enabled = $True
$PrimaryDNSTextBox.Enabled = $True
$IPAddressTextBox.Enabled = $True

    # If there is a Default Gatway already configured, display that
    if($Global:DefaultGateways[$IPSchemaListBox.SelectedIndex]) {
    
    $DefaultGatewayTextBox.Text = $Global:DefaultGateways[$IPSchemaListBox.SelectedIndex]
    
    }
   
    # Else, display and set the Default Gateway from the previously selected index, but only if there is no previsouly set Default Gateway
    elseif($Global:DefaultGateways[$Global:PreviouslySelectedIndex]) {
    
    $Global:DefaultGateways[$IPSchemaListBox.SelectedIndex] = $Global:DefaultGateways[$Global:PreviouslySelectedIndex]
    
    }

#####################################################################################################################################################################
    
    # If there is a Subnet Mask already configured, display that
    if($Global:SubnetMasks[$IPSchemaListBox.SelectedIndex]) {
    
    $SubnetMaskTextBox.Text = $Global:SubnetMasks[$IPSchemaListBox.SelectedIndex]

    }

    # Else, display and set the Subnet Mask from the previously selected index, but only if there is no previsouly set Subnet Mask
    elseif($Global:SubnetMasks[$Global:PreviouslySelectedIndex]) {
    
    $Global:SubnetMasks[$IPSchemaListBox.SelectedIndex] = $Global:SubnetMasks[$Global:PreviouslySelectedIndex]

    }

#####################################################################################################################################################################
    
    # If there is a Primary DNS Server already configured, display that
    if($Global:PrimaryDNSServers[$IPSchemaListBox.SelectedIndex]) {
    
    $PrimaryDNSTextBox.Text = $Global:PrimaryDNSServers[$IPSchemaListBox.SelectedIndex]

    }

    # Else, display and set the Primary DNS Server from the previously selected index, but only if there is no previsouly set Primary DNS Server
    elseif($Global:PrimaryDNSServers[$Global:PreviouslySelectedIndex]) {
    
    $Global:PrimaryDNSServers[$IPSchemaListBox.SelectedIndex] = $Global:PrimaryDNSServers[$Global:PreviouslySelectedIndex]

    }

#####################################################################################################################################################################
    
    # If there is a Secondary DNS Server already configured, display that
    if($Global:SecondaryDNSServers[$IPSchemaListBox.SelectedIndex]) {
    
    $SecondaryDNSTextBox.Text = $Global:SecondaryDNSServers[$IPSchemaListBox.SelectedIndex]

    }
    
    # Else, display and set the Secondary DNS Server from the previously selected index, but only if there is no previsouly set Secondary DNS Server
    elseif($Global:SecondaryDNSServers[$Global:PreviouslySelectedIndex]) {
    
    $Global:SecondaryDNSServers[$IPSchemaListBox.SelectedIndex] = $Global:SecondaryDNSServers[$Global:PreviouslySelectedIndex]

    }

#####################################################################################################################################################################

    # If there is not an IP Address already configured for the selected index, take the first 3 octets of the previsously selected indexes configured IP Address, if there is one 
    if(($Global:IPAddresses[$IPSchemaListBox.SelectedIndex] -notmatch [regex]"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$") -and ($Global:IPAddresses[$Global:PreviouslySelectedIndex]) -and ($Global:IPAddresses[$Global:PreviouslySelectedIndex] -notmatch [regex]"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}$")) {

    $IPAddressTextBox.Text = $Global:IPAddresses[$Global:PreviouslySelectedIndex].Remove($Global:IPAddresses[$Global:PreviouslySelectedIndex].LastIndexOf(".")+1)
    
    }

    # Else, if there is already an IP configured for the selected index, display that
    elseif($Global:IPAddresses[$IPSchemaListBox.SelectedIndex] -match [regex]"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"){
    
    $IPAddressTextBox.Text = $Global:IPAddresses[$IPSchemaListBox.SelectedIndex]
    
    }

}


Function Convert-IpAddressToMaskLength([string] $DottedIpAddressString) {
$NetMaskResult = 0
[IPAddress] $BaseNetMask = $DottedIpAddressString
$Octets = $BaseNetMask.IPAddressToString.Split('.')

    foreach($Octet in $Octets) {

        while(0 -ne $Octet) {

            $Octet = ($Octet -shl 1) -band [byte]::MaxValue
            $NetMaskResult++

        }

    }

  return $NetMaskResult

}


Function StartCheckJobs {

$XenServerHost = $XenHostTextBox.Text
$XenServerUsername = $UsernameTextBox.Text
$XenServerPassword = $PasswordTextBox.Text

    foreach($UpStateServer in ($Global:AllCreatedServers | sort)) {

        $Global:GetVMIPs += Start-Job -ScriptBlock {

        Import-Module $args[1]

        Connect-XenServer -Url "https://$($args[2])" -UserName $args[3] -Password $args[4] -NoWarnCertificates -SetDefaultSession

            While(!(Get-XenVM -Name $args[0] | Select -ExpandProperty guest_metrics | Get-XenVMGuestMetrics).networks.ContainsKey("0/ipv4/0")) {

                if((Get-XenVM -Name $args[0] | Select -ExpandProperty guest_metrics | Get-XenVMGuestMetrics).networks.ContainsKey("0/ipv4/0")) {

                break

                }

            WaitScript 2

            }

        } -ArgumentList $UpStateServer, $Global:XenServerModule, $XenServerHost, $XenServerUsername, $XenServerPassword -Name $UpStateServer

    }

}


Function CheckServerIPState {

$RunningCount = 0
[System.Collections.ArrayList]$NeededReboot = @()
$ListViewCount = $CheckStateListView.Items.Count

    while($NeededReboot.Count -ne $Global:AllCreatedServers.Count) {
    
    $NeededReboot += $False

    }

$ServerStatusLabel.Text = "Waiting for Servers to Obtain IP Addresses: $RunningCount/$ListViewCount Complete `n`nNote: This Can Take up to 1.5 Hours to Complete`nElapsed Time: $($Global:StopWatch.Elapsed.Hours):$($Global:StopWatch.Elapsed.Minutes):$($Global:StopWatch.Elapsed.Seconds)"

    While($RunningCount -lt $ListViewCount) {

        foreach($RunningJob in $Global:GetVMIPs) {

            if(((Get-XenVM -Name $RunningJob.Name).allowed_operations -contains "clean_reboot") -and (!((Get-XenVM -Name $RunningJob.Name | Select -ExpandProperty guest_metrics | Get-XenVMGuestMetrics).networks.'0/ipv4/0')) -and ($NeededReboot[($Global:AllCreatedServers | sort).IndexOf($RunningJob.Name)] -eq $False)) {

            $NeededReboot[($Global:AllCreatedServers | sort).IndexOf($RunningJob.Name)] = $True

                $RebootServer = Start-Job -ScriptBlock {
            
                param ($RunningJob,$XenServerModule,$XenHost,$Username,$Password)

                Import-Module $XenServerModule

                Connect-XenServer -Url "https://$XenHost" -UserName $Username -Password $Password -NoWarnCertificates -SetDefaultSession

                Invoke-XenVM -Name $RunningJob -XenAction CleanReboot
            
                } -ArgumentList $RunningJob.Name,$Global:XenServerModule,$XenHostTextBox.Text,$UsernameTextBox.Text,$PasswordTextBox.Text

            }

            if((Get-Job $RunningJob.Name -ErrorAction SilentlyContinue).State -eq "Completed" -and ((Get-XenVM -Name $RunningJob.Name | Select -ExpandProperty guest_metrics | Get-XenVMGuestMetrics).networks.'0/ipv4/0') -and ((Get-XenVM -Name $RunningJob.Name | Select -ExpandProperty guest_metrics | Get-XenVMGuestMetrics).networks.'0/ipv4/0') -notmatch "169.254") {
            
            $RunningCount++
            $CurrentIPAddress = (Get-XenVM -Name $RunningJob.Name | Select -ExpandProperty guest_metrics | Get-XenVMGuestMetrics).networks.'0/ipv4/0'
            $CheckStateListView.Items.RemoveAt($CheckStateListView.Items.Text.IndexOf($RunningJob.Name))

            $ListViewItem = New-Object System.Windows.Forms.ListViewItem($RunningJob.Name)
            $ListViewItem.Subitems.Add("Completed") | Out-Null
            $ListViewItem.Subitems.Add($CurrentIPAddress) | Out-Null
            $CheckStateListView.Items.Add($ListViewItem) | Out-Null

            $CheckStateListView.Sorting = "Ascending"

                if(Get-Job $RunningJob.Name -ErrorAction SilentlyContinue) {

                Remove-Job -Name $RunningJob.Name -Force

                }

            $ServerStatusLabel.Text = "Waiting for Servers to Obtain IP Addresses: $RunningCount/$ListViewCount Complete `n`nNote: This Can Take up to 1.5 Hours to Complete`nElapsed Time: $($Global:StopWatch.Elapsed.Hours):$($Global:StopWatch.Elapsed.Minutes):$($Global:StopWatch.Elapsed.Seconds)"

            }

            elseif((Get-Job $RunningJob.Name -ErrorAction SilentlyContinue).State -eq "Failed"){
            
            $RunningCount++
            $CheckStateListView.Items.RemoveAt($CheckStateListView.Items.Text.IndexOf($RunningJob.Name))

            $ListViewItem = New-Object System.Windows.Forms.ListViewItem($RunningJob.Name)
            $ListViewItem.Subitems.Add("Failed") | Out-Null
            $ListViewItem.Subitems.Add("Unknown") | Out-Null
            $CheckStateListView.Items.Add($ListViewItem) | Out-Null

            $CheckStateListView.Sorting = "Ascending"

                if(Get-Job $RunningJob.Name -ErrorAction SilentlyContinue) {

                Remove-Job -Name $RunningJob.Name -Force

                }

            $ServerStatusLabel.Text = "Waiting for Servers to Obtain IP Addresses: $RunningCount/$ListViewCount Complete `n`nNote: This Can Take up to 1.5 Hours to Complete`nElapsed Time: $($Global:StopWatch.Elapsed.Hours):$($Global:StopWatch.Elapsed.Minutes):$($Global:StopWatch.Elapsed.Seconds)"

            }

        $ServerStatusLabel.Text = "Waiting for Servers to Obtain IP Addresses: $RunningCount/$ListViewCount Complete `n`nNote: This Can Take up to 1.5 Hours to Complete`nElapsed Time: $($Global:StopWatch.Elapsed.Hours):$($Global:StopWatch.Elapsed.Minutes):$($Global:StopWatch.Elapsed.Seconds)"
        $ServerStatusLabel.Refresh()

        }

    Start-Sleep -Milliseconds 200
    [System.Windows.Forms.Application]::DoEvents()
    $ServerStatusLabel.Text = "Waiting for Servers to Obtain IP Addresses: $RunningCount/$ListViewCount Complete `n`nNote: This Can Take up to 1.5 Hours to Complete`nElapsed Time: $($Global:StopWatch.Elapsed.Hours):$($Global:StopWatch.Elapsed.Minutes):$($Global:StopWatch.Elapsed.Seconds)"
    $ServerStatusLabel.Refresh()

    }

$Global:StopWatch.Stop()
WaitScript 5

}


Function DomainCreation {

$ServerStatusLabel.Text = "Starting Domain Creation `n`nNote: This Will Take About 15-25 Minutes to Complete`n"

$ProccessFullReboot = 0

$VMStatusTextBox.AppendText("`r`n=========================")
$VMStatusTextBox.AppendText("`r`nStarting Domain Creation")
$VMStatusTextBox.AppendText("`r`n=========================`r`n")

WaitScript 10

AssociateVMsWithOldIPs

WaitScript 10

ChangeIPAddresses

$VMStatusTextBox.AppendText("`r`nWating for All IP Addresses to Change")

    foreach($ComputerIP in $Global:IPAddresses) {
    
    CheckServerOnState $ComputerIP
    
    }

$VMStatusTextBox.AppendText("`r`nAll IP Addresses Have Been Successfully Changed`r`n")

ChangeComputerNames

WaitScript 10

$VMStatusTextBox.AppendText("`r`nWating for All VMs to Reboot")

    foreach($DCCreated in $DomainControllersListBox.Items) {
   
        if($DCCreated -match [regex]'\*') {
            
        $DCCreated = $DCCreated.Replace("*","")
            
        }

    CheckServerOnState $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DCCreated)]

    }

$VMStatusTextBox.AppendText("`r`nAll VMs Have Finished Rebooting`r`n")

WaitScript 20

InstallDCComponents
 
WaitScript 10

PromotePrimaryDomainController

WaitScript 10

    foreach($PrimaryDCCreated in $DomainControllersListBox.Items  | where {$_ -match [regex]'\*'}) {

    CheckServerOnState $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($PrimaryDCCreated.Replace("*",""))]

    }

$VMStatusTextBox.AppendText("`r`nWating for $(($DomainControllersListBox.Items  | where {$_ -match [regex]'\*'}).Replace("*",'')) to Completely Reboot`r`n")

    while($ProccessFullReboot -ne 6){
    
    $VMStatusTextBox.AppendText("`r`nTime Until Resumption: $(6 - $ProccessFullReboot) Minutes")

    WaitScript 59

    $ProccessFullReboot++

    }

WaitScript 10

PromoteSecondaryDomainControllers

WaitScript 10

    foreach($SecondaryDCCreated in $DomainControllersListBox.Items | where {$_ -notmatch [regex]'\*'}) {

    CheckServerOnState $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($SecondaryDCCreated)]

    }

WaitScript 40

JoinToDomain

WaitScript 10

$VMStatusTextBox.AppendText("`r`nWaiting for all VMs to Reboot`r`n")

    foreach($DomainServer in $AllServersListBox.Items) {

    CheckServerOnState $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DomainServer)]

    }

WaitScript 10

$VMStatusTextBox.AppendText("`r`n=========================")
$VMStatusTextBox.AppendText("`r`nDomain Creation Complete")
$VMStatusTextBox.AppendText("`r`n=========================`r`n")

}


Function ValidateDomainCreation {

$DomainValidationCount = 0
$WrongDefaultGateway = @()
$WrongSubnetMask = @()
$WrongIPAddress = @()
$WrongPrimaryDNS = @()
$WrongSecondaryDNS = @()
$PrimaryDC = @()
$DomainCounter = 0


    if($LocalUsernameTextBox.Text.Length -lt 1) {
    
    [System.Windows.MessageBox]::Show("Error in Local Username TextBox: No user name provided")
    $DomainBuildoutButton.Enabled = $False

    }else{ $DomainValidationCount++ }


    if($LocalPasswordTextBox.Text.Length -lt 1) {
    
    [System.Windows.MessageBox]::Show("Error in Local Password TextBox: No password provided")
    $DomainBuildoutButton.Enabled = $False

    }else{ $DomainValidationCount++ }


    if($DomainNameTextBox.Text.Length -lt 1) {
    
    [System.Windows.MessageBox]::Show("Error in Domain Name TextBox: No Domain name provided")
    $DomainBuildoutButton.Enabled = $False

    }

    elseif($DomainNameTextBox.Text -notmatch [regex]"^(?!:\/\/)([a-zA-Z0-9-_]+\.)*[a-zA-Z0-9][a-zA-Z0-9-_]+\.[a-zA-Z]{2,11}?$") {
    
    [System.Windows.MessageBox]::Show("Error in Domain Name TextBox: Syntax error")
    $DomainBuildoutButton.Enabled = $False
    
    }else{ $DomainValidationCount++ }


    if($SafeModePasswordTextBox.Text.Length -lt 1) {
    
    [System.Windows.MessageBox]::Show("Error in Safe Mode Password TextBox: No safe mode password provided")
    $DomainBuildoutButton.Enabled = $False

    }

    elseif($SafeModePasswordTextBox.Text -notmatch [regex]"(?=(.*[0-9]))(?=.*[\!@#$%^&*()\\[\]{}\-_+=~`|:;'<>,.?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}") { 
    
    [System.Windows.MessageBox]::Show("Error in Safe Mode Password TextBox: Syntax error`n`nSafe Mode password must meet the following:`n`t- At least 8 characters`n`t- At least one lowercase letter`n`t- At least one uppercase letter`n`t- At least one number`n`t- At least one non-alpha numeric character")
    $DomainBuildoutButton.Enabled = $False
    
    }else{$DomainValidationCount++ }


    foreach($DomainServer in $IPSchemaListBox.Items) {
    
        if($Global:DefaultGateways[$DomainCounter] -notmatch [regex]"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$") {
        
        $WrongDefaultGateway += "`t- $($IPSchemaListBox.Items[$DomainCounter])`n"

        }

        if($Global:SubnetMasks[$DomainCounter] -notmatch [regex]"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$") {
        
        $WrongSubnetMask += "`t- $($IPSchemaListBox.Items[$DomainCounter])`n"

        }

        if($Global:IPAddresses[$DomainCounter] -notmatch [regex]"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$") {
        
        $WrongIPAddress += "`t- $($IPSchemaListBox.Items[$DomainCounter])`n"

        }

        if($Global:PrimaryDNSServers[$DomainCounter] -notmatch [regex]"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$") {
        
        $WrongPrimaryDNS += "`t- $($IPSchemaListBox.Items[$DomainCounter])`n"

        }

        if($Global:SecondaryDNSServers[$DomainCounter] -ne $null){

            if($Global:SecondaryDNSServers[$DomainCounter] -notmatch [regex]"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$") {
        
            $WrongSecondaryDNS += "`t- $($IPSchemaListBox.Items[$DomainCounter])`n"

            }

        }
    
    $DomainCounter++
    }


    if($WrongDefaultGateway -or $WrongSubnetMask -or $WrongIPAddress -or $WrongPrimaryDNS -or $WrongSecondaryDNS) {
    
    [System.Windows.MessageBox]::Show("Error For the Following Server IP Configurations:`n`nDefault Gateway:`n`n$WrongDefaultGateway`n`nSubnet Mask:`n`n$WrongSubnetMask`n`nIP Address:`n`n$WrongIPAddress`n`nPrimary DNS:`n`n$WrongPrimaryDNS`n`nSecondary DNS:`n`n$WrongSecondaryDNS")
    $DomainBuildoutButton.Enabled = $False
    
    }else{ $DomainValidationCount++ }


    foreach($PrimaryController in $DomainControllersListBox.Items) {
    
        if($PrimaryController -match [regex]"\*") {
        
        $PrimaryDC += $PrimaryController
        
        }
    
    }


    if(!$DomainControllersListBox.Items) {
    
    [System.Windows.MessageBox]::Show("Error in Domain Controller Selection: No Domain Controller(s) selected")
    $DomainBuildoutButton.Enabled = $False 
    
    }

    elseif($DomainControllersListBox.Items.Count -ge 2 -and !$PrimaryDC) {
    
    [System.Windows.MessageBox]::Show("Error in Domain Controller Selection: No primary Domain Controller selected")
    $DomainBuildoutButton.Enabled = $False
    
    }else{ $DomainValidationCount++ }

    
    if($DomainValidationCount -eq 6) {
    
    $DomainBuildoutButton.Enabled = $True
    
    }

}


#################################  USER - GROUP - OU BUILDOUT  #################################

Function PopulateOUDropDowns {

    if($DropDownParentOU.Items) {

    $DropDownParentOU.Items.Clear()

    }

    if($DropDownGroupOU.Items) {

    $DropDownGroupOU.Items.Clear()

    }

    if($DropDownUsersOU.Items) {

    $DropDownUsersOU.Items.Clear()

    }

    Foreach($ListedOU in $OUStructureListBox.Items) {

    $DropDownParentOU.Items.Add($ListedOU.Substring($ListedOU.IndexOf("-")+1))  
            
    }

    Foreach($ListedOU in $OUStructureListBox.Items | Where {$_.Substring($_.IndexOf("-")+1) -notmatch $DomainNameTextBox.Text}) {
    
    $DropDownGroupOU.Items.Add($ListedOU.Substring($ListedOU.IndexOf("-")+1))
    $DropDownUsersOU.Items.Add($ListedOU.Substring($ListedOU.IndexOf("-")+1))
    
    } 

    

}


Function PopulateGroupDropDowns {

$GroupScopes = @("DomainLocal", "Global", "Universal")
$GroupTypes = @("Security", "Distribution")

    Foreach($GroupScope in $GroupScopes) {

    $DropDownGroupScope.Items.Add($GroupScope)        
            
    }

    Foreach($GroupType in $GroupTypes) {
    
    $DropDownGroupType.Items.Add($GroupType)
    
    } 

}


Function AddOU {

$ParentOU = $DropDownParentOU.Text
$AllOUs = @()

    foreach($OUName in $OUStructureListBox.Items) {
    
    $AllOUs += $OUName.SubString($OUName.IndexOf("-")+1)
    
    }
    
    if(((($Global:OULocation | where {$_ -match $ParentOU} | select -Skip 1) | % {$_.Remove($_.IndexOf(",")).SubString($_.IndexOf("=")+1)}) -notcontains $OUNameTextBox.Text) -and (($Global:OULocation | where {$_ -ne $Null -and $_.Split(",",2)[1] -match $Global:BaseDN} | % {if($ParentOU -eq $DomainNameTextBox.Text){$_.Remove($_.IndexOf(",")).SubString($_.IndexOf("=")+1)}}) -notcontains $OUNameTextBox.Text) -and $OUNameTextBox.Text -notmatch $DomainNameTextBox.Text) {
            
        if($DropDownParentOU.Text -notmatch $DomainNameTextBox.Text) {

        $OUStructureListBox.Items.Insert($AllOUs.IndexOf($DropDownParentOU.Text)+1,"$($OUStructureListBox.Text.Remove($OUStructureListBox.Text.IndexOf("-")))   -$($OUNameTextBox.Text)")
        $Global:AllCreatedOUs += $Null
        $Global:AllCreatedOUs.Insert($AllOUs.IndexOf($DropDownParentOU.Text)+1,"$($OUStructureListBox.Text.Remove($OUStructureListBox.Text.IndexOf("-")))   -$($OUNameTextBox.Text)")

        }

        else {

        $OUStructureListBox.Items.Insert($OUStructureListBox.Items.IndexOf($DropDownParentOU.Text)+1,"   -$($OUNameTextBox.Text)")
        $Global:AllCreatedOUs += $Null
        $Global:AllCreatedOUs.Insert($OUStructureListBox.Items.IndexOf($DropDownParentOU.Text)+1,"   -$($OUNameTextBox.Text)")
           
        }

        while($Global:OULocation.Count -ne $OUStructureListBox.Items.Count) {
            
        $Global:OULocation += $Null
            
        }

        Foreach($OU in $OUNameTextBox.Text) {
            
            if($ParentOU -notmatch $DomainNameTextBox.Text){
                
            $Global:OULocation.Insert($Global:OULocation.IndexOf(($Global:OULocation | where {$_ -match $ParentOU} | select -First 1))+1,"OU=$($OU.Substring($OU.IndexOf("-")+1)),$($Global:OULocation | where {$_ -match $ParentOU} | select -First 1)")

            }

            else {
                
            $Global:OULocation.Insert($Global:OULocation.LastIndexOf($Global:BaseDN)+1,"OU=$($OU.Substring($OU.IndexOf("-")+1)),$Global:BaseDN")
                
            }

        }

    }

    else{
    
    [System.Windows.MessageBox]::Show("Error in OU Name TextBox: The OU you are trying to add is already been added for that OU level")
    
    }

}


Function AddGroup {
            
$AddGroupValidation = 0 

    if($GroupNameTextBox.Text.Length -lt 1) {
            
    [System.Windows.MessageBox]::Show("Error in Group Name TextBox: No group name input")
            
    } 
            
    elseif($Global:GroupName[0..$Global:GroupName.Count] | Select-String $GroupNameTextBox.Text) {

    [System.Windows.MessageBox]::Show("Error in Group Name TextBox: Group already exists, choose a different name")

    } else { $AddGroupValidation++ }

    if(!($DropDownGroupOU.SelectedItem)) {
            
    [System.Windows.MessageBox]::Show("Error in OU Location DropDown: No OU selected")
            
    } else { $AddGroupValidation++ }

    if(!($DropDownGroupScope.SelectedItem)) {
            
    [System.Windows.MessageBox]::Show("Error in Group Scope DropDown: No group scope selected")
            
    } else { $AddGroupValidation++ }

    if(!($DropDownGroupType.SelectedItem)) {
            
    [System.Windows.MessageBox]::Show("Error in Group Type DropDown: No group type selected")
            
    } else { $AddGroupValidation++ }

    if($AddGroupValidation -eq 4) {

    $GroupsListBox.Items.Add($GroupNameTextBox.Text)

    $Global:GroupName += $GroupNameTextBox.Text
    $Global:GroupOULocation += $DropDownGroupOU.Text
    $Global:GroupScope += $DropDownGroupScope.Text
    $Global:GroupType += $DropDownGroupType.Text  
                
    $GroupNameTextBox.Clear()
    $DropDownGroupOU.ResetText()
    $DropDownGroupScope.ResetText()
    $DropDownGroupType.ResetText()

    }

}


Function AddUser {

$AddUserValidation = 0

    if($LoginNameTextBox.Text.Length -lt 1) {
            
    [System.Windows.MessageBox]::Show("Error in User Login Name TextBox: No login name input")
            
    } 
            
    elseif($Global:UserLoginName[0..$Global:UserLoginName.Count] | Select-String $LoginNameTextBox.Text) {

    [System.Windows.MessageBox]::Show("Error in User Login Name TextBox: Login name already exists, choose a different name")

    } else { $AddUserValidation++ }   

    if($FirstNameTextBox.Text.Length -lt 1) {
            
    [System.Windows.MessageBox]::Show("Error in First Name TextBox: No first name input")
            
    } else { $AddUserValidation++ }

    if($LastNameTextBox.Text.Length -lt 1) {
            
    [System.Windows.MessageBox]::Show("Error in Last Name TextBox: No last name input")
            
    } else { $AddUserValidation++ }

    if($UserPasswordTextBox.Text -notmatch [regex]"(?=(.*[0-9]))(?=.*[\!@#$%^&*()\\[\]{}\-_+=~`|:;'<>,.?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}") { 
    
    [System.Windows.MessageBox]::Show("Error in User Password TextBox: Syntax error`n`nUser password must meet the following:`n`t- At least 8 characters`n`t- At least one lowercase letter`n`t- At least one uppercase letter`n`t- At least one number`n`t- At least one non-alpha numeric character")
    
    } else { $AddUserValidation++ }

    if(!($DropDownUsersOU.SelectedItem)) {
            
    [System.Windows.MessageBox]::Show("Error in User OU Location DropDown: No OU selected")
            
    } else { $AddUserValidation++ }

    if($AddUserValidation -eq 5) {
                
    $UsersListBox.Items.Add("$($FirstNameTextBox.Text) $($LastNameTextBox.Text)")

    $Global:UserLoginName += $LoginNameTextBox.Text
    $Global:UserFirstName += $FirstNameTextBox.Text
    $Global:UserLastName += $LastNameTextBox.Text
    $Global:UserPassword += $UserPasswordTextBox.Text
    $Global:UserOULocation += $DropDownUsersOU.Text
                
    $LoginNameTextBox.Clear()
    $FirstNameTextBox.Clear()
    $LastNameTextBox.Clear()
    $UserPasswordTextBox.Clear()
    $DropDownUsersOU.ResetText()

    }

}


Function CreateOUs {

$PrimaryDC = ($DomainControllersListBox.Items | where {$_ -match [regex]'\*'})
$DomainName = $DomainNameTextBox.Text
   
    foreach($OULocation in ($Global:OULocation | where {$_ -ne $Null})) {

    $ConnectionPassword = ConvertTo-SecureString -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $DomainAdminCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

    $VMStatusTextBox.AppendText("`r`nCreating $($OULocation.Remove($OULocation.IndexOf(",")).Substring($OULocation.IndexOf("=")+1)) OU")

        Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($PrimaryDC.Replace("*",""))] -credential $DomainAdminCreds -ScriptBlock {

        param ($OULocation)

        New-ADOrganizationalUnit -Name ($OULocation.Remove($OULocation.IndexOf(",")).Substring($OULocation.IndexOf("=")+1)) -Path $OULocation.Substring($OULocation.IndexOf(",")+1)

        } -ArgumentList $OULocation

    Start-Sleep -Milliseconds 500
    [System.Windows.Forms.Application]::DoEvents()

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function CreateGroups {

$PrimaryDC = ($DomainControllersListBox.Items | where {$_ -match [regex]'\*'})
$DomainName = $DomainNameTextBox.Text
    
    foreach($CreatedGroup in $GroupsListBox.Items) {

    $ConnectionPassword = convertto-securestring -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $DomainAdminCreds = new-object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

    $VMStatusTextBox.AppendText("`r`nCreating Group - $CreatedGroup")

        Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($PrimaryDC.Replace("*",""))] -credential $DomainAdminCreds -ScriptBlock {

        param ($GroupName,$GroupOULocation,$GroupScope,$GroupType)

        New-ADGroup -Name $GroupName -GroupScope $GroupScope -GroupCategory $GroupType -path $GroupOULocation

        } -ArgumentList $Global:GroupName[$GroupsListBox.Items.IndexOf($CreatedGroup)],(($Global:OULocation | where {$_ -match $Global:GroupOULocation[$GroupsListBox.Items.IndexOf($CreatedGroup)]}) | Select -First 1),$Global:GroupScope[$GroupsListBox.Items.IndexOf($CreatedGroup)],$Global:GroupType[$GroupsListBox.Items.IndexOf($CreatedGroup)]

    Start-Sleep -Milliseconds 500
    [System.Windows.Forms.Application]::DoEvents()
    
    }
    
$VMStatusTextBox.AppendText("`r`n")

}


Function CreateUsers {

$PrimaryDC = ($DomainControllersListBox.Items | where {$_ -match [regex]'\*'})
$DomainName = $DomainNameTextBox.Text
    
    foreach($CreatedUser in $UsersListBox.Items) {

    $ConnectionPassword = ConvertTo-SecureString -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $DomainAdminCreds = New-Object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

    $VMStatusTextBox.AppendText("`r`nCreating User - $CreatedUser")

        Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($PrimaryDC.Replace("*",""))] -credential $DomainAdminCreds -ScriptBlock {

        param ($LoginName,$FirstName,$LastName,$UserPassword,$UserOULocation,$DomainName)

        New-ADUser -Name "$FirstName $LastName" -GivenName $FirstName -Surname $LastName -DisplayName "$FirstName $LastName" -UserPrincipalName "$LoginName@$DomainName" -Enabled $True -AccountPassword (ConvertTo-SecureString -AsPlainText -Force -String $UserPassword) -Path $UserOULocation

        } -ArgumentList $Global:UserLoginName[$UsersListBox.Items.IndexOf($CreatedUser)],$Global:UserFirstName[$UsersListBox.Items.IndexOf($CreatedUser)],$Global:UserLastName[$UsersListBox.Items.IndexOf($CreatedUser)],$Global:UserPassword[$UsersListBox.Items.IndexOf($CreatedUser)],(($Global:OULocation | where {$_ -match $Global:UserOULocation[$UsersListBox.Items.IndexOf($CreatedUser)]}) | Select -First 1),$DomainName

    Start-Sleep -Milliseconds 500
    [System.Windows.Forms.Application]::DoEvents()

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function UGOCreation {

$ServerStatusLabel.Text = "Starting User, Group, and OU Creation `n`nNote: This Will Take About 1-5 Minutes to Complete"

$VMStatusTextBox.AppendText("`r`n=========================")
$VMStatusTextBox.AppendText("`r`nStarting User, Group, and OU Creation")
$VMStatusTextBox.AppendText("`r`n=========================`r`n")

CreateOUs

WaitScript 2

CreateGroups

WaitScript 2

CreateUsers

$VMStatusTextBox.AppendText("`r`n=========================")
$VMStatusTextBox.AppendText("`r`nUser, Group, and OU Creation Complete")
$VMStatusTextBox.AppendText("`r`n=========================`r`n")

}


#################################  CERTIFICATE SERVICES BUILDOUT  #################################

Function FillCertificateDropDowns {

$CryptoProviders = @("Microsoft Base Smart Card Crypto Provider", "Microsoft Enhanced Cryptographic Provider v1.0", "ECDA_P256#Microsoft Smart Card Key Storage Provider", "ECDA_P521#Microsoft Smart Card Key Storage Provider", "RSA#Microsoft Software Key Storage Provider","Microsoft Base Cryptographic Provider v1.0", "ECDA_P256#Microsoft Software Key Storage Provider", "ECDA_P521#Microsoft Software Key Storage Provider", "Microsoft Strong Cryptographic Provider", "ECDA_P384#Microsoft Software Key Storage Provider", "Microsoft Base DSS Cryptographic Provider", "RSA#Microsoft Smart Card Key Storage Provider", "DSA#Microsoft Software Key Storage Provider", "ECDA_P384#Microsoft Smart Card Key Storage Provider")
$KeyLengths = @(512, 1024, 2048, 4096)
$CATypes = @("EnterpriseRootCA", "EnterpriseSubordinateCA", "StandaloneRootCA", "StandaloneSubordinateCA")
$ValidityPeriods = @("Hours", "Days", "Weeks", "Months", "Years")
$HashAlgorithms = @("SHA256", "SHA384", "SHA512", "SHA1", "MD5", "MD4", "MD2")
$ValidityUnits = 1

    $DropDownCryptoProvider.Items.Clear()

    foreach($CryptoProvider in $CryptoProviders) {
    
    $DropDownCryptoProvider.Items.Add($CryptoProvider) | Out-Null
    
    }

    $DropDownCryptoProvider.SelectedItem = "RSA#Microsoft Software Key Storage Provider"

    $DropDownKeyLength.Items.Clear()

    foreach($KeyLength in $KeyLengths) {
    
    $DropDownKeyLength.Items.Add($KeyLength) | Out-Null
    
    }

    $DropDownKeyLength.SelectedItem = 2048

    $DropDownCAType.Items.Clear()

    foreach($CAType in $CATypes) {
    
    $DropDownCAType.Items.Add($CAType) | Out-Null
    
    }

    $DropDownValidityPeriod.Items.Clear()

    foreach($ValidityPeriod in $ValidityPeriods) {
    
    $DropDownValidityPeriod.Items.Add($ValidityPeriod) | Out-Null
    
    }

    $DropDownValidityPeriod.SelectedItem = "Years"

    $DropDownHashAlgorithm.Items.Clear()

    foreach($HashAlgorithm in $HashAlgorithms) {
    
    $DropDownHashAlgorithm.Items.Add($HashAlgorithm) | Out-Null
    
    }

    $DropDownHashAlgorithm.SelectedItem = "SHA256"

    $DropDownValidityPeriodUnits.Items.Clear()

    While($ValidityUnits -lt 51) {
    
    $DropDownValidityPeriodUnits.Items.Add($ValidityUnits) | Out-Null
    $ValidityUnits++

    }

    $DropDownValidityPeriodUnits.SelectedItem = 5

}


Function InstallCertificateComponents {

$CertificateCounter = 0

    foreach($CAServer in $CertificateAuthoritiesListBox.Items){

    $DomainName = $DomainNameTextBox.Text
    $ConnectionPassword = convertto-securestring -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $DomainAdminCreds = new-object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

    $VMStatusTextBox.AppendText("`r`nInstalling Certificate Services Components on $CAServer")

        $CertificatePromotion = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($CAServer)] -credential $DomainAdminCreds -ScriptBlock {

        Install-WindowsFeature Adcs-Cert-Authority -IncludeManagementTools

        } -AsJob

        WaitJob $CertificatePromotion

        if($Global:CAWebEnrollment[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)] -eq "Checked") {

        $VMStatusTextBox.AppendText("`r`nInstalling AD CS Web Enrollment Components on $CAServer")
    
            $WebEnrollment = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($CAServer)] -credential $DomainAdminCreds -ScriptBlock {

            Install-WindowsFeature ADCS-Web-Enrollment -IncludeManagementTools

            } -AsJob

            WaitJob $WebEnrollment
    
        }

        if($Global:CAResponder[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)] -eq "Checked") {

        $VMStatusTextBox.AppendText("`r`nInstalling AD CS Online Responder Components on $CAServer")
    
            $Responder = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($CAServer)] -credential $DomainAdminCreds -ScriptBlock {

            Install-WindowsFeature ADCS-Online-Cert -IncludeManagementTools

            } -AsJob

            WaitJob $Responder
    
        }

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function InstallAllServices {

$NonSubordinates = @()
$Subordinates = @()
$AllCAServers = @()
    
    #Fill arrays with Specified Certificate Authorities
    foreach($CAServer in $CertificateAuthoritiesListBox.Items){

        if($Global:CATypes[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)] -notmatch "Subordinate") {

        $NonSubordinates += $CAServer

        }

        else {
        
        $Subordinates += $CAServer
        
        }

    }

    #Fill primary array starting with all non-subordinate CAs
    foreach($NonSubordinate in $NonSubordinates) {
    
    $AllCAServers += $NonSubordinate
    
    }

    #Next, fill primary array with all subordinate CAs
    foreach($Subordinate in $Subordinates) {
    
    $AllCAServers += $Subordinate
    
    }

    foreach($CAServer in $AllCAServers){

    #Define necessary connection parameters
    $DomainName = $DomainNameTextBox.Text
    $ConnectionPassword = convertto-securestring -AsPlainText -Force -String $LocalPasswordTextBox.Text
    $DomainAdminCreds = new-object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

    $VMStatusTextBox.AppendText("`r`nPromoting $CAServer to a Certificate Authority")
        
        #If the server is not a subordinate CA, define all parameters
        if($Global:CATypes[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)] -notmatch "Subordinate") {

            $RootCA = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($CAServer)] -credential $DomainAdminCreds -ScriptBlock {

            param ($CAType, $CAName, $HashAlgorithm, $KeyLength, $CryptoProvider, $ValidityPeriod, $ValidityPeriodUnits, $DomainAdminCreds, $DomainName)

            Install-AdcsCertificationAuthority -CAType $CAType -CACommonName $CAName -HashAlgorithmName $HashAlgorithm -KeyLength $KeyLength  -CryptoProviderName $CryptoProvider -ValidityPeriod $ValidityPeriod -ValidityPeriodUnits $ValidityPeriodUnits -Credential $DomainAdminCreds -Confirm:$False
            
            } -ArgumentList $Global:CATypes[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $Global:CANames[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $Global:CAHashAlgorithm[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $Global:CAKeyLength[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $Global:CACryptoProvider[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $Global:CAValidityPeriod[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $Global:CAValidityPeriodUnits[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $DomainAdminCreds, $DomainName -AsJob

            WaitJob $RootCA

        }

        #Else, only create a CA using the parent specified and a few other parameters
        else {

            $SubordinateCA = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($CAServer)] -credential $DomainAdminCreds -ScriptBlock {

            param ($CAType, $CAName, $ParentCAName, $ParentCA, $DomainAdminCreds, $DomainName)

            Install-AdcsCertificationAuthority -CAType $CAType -ParentCA "$ParentCA.$DomainName\$ParentCAName" -CACommonName $CAName -Credential $DomainAdminCreds -Confirm:$False

            } -ArgumentList $Global:CATypes[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $Global:CANames[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $Global:CANames[$CertificateAuthoritiesListBox.Items.IndexOf($Global:ParentCA[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)])], $Global:ParentCA[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)], $DomainAdminCreds, $DomainName -AsJob

            WaitJob $SubordinateCA

        } 
        
        #If the server was chosen as a web enrollment server, install the role
        if($Global:CAWebEnrollment[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)] -eq "Checked") {

        $VMStatusTextBox.AppendText("`r`nPromoting $CAServer to a Web Enrollment Server")
    
            $EnrollmentPromotion = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($CAServer)] -credential $DomainAdminCreds -ScriptBlock {

            Install-AdcsWebEnrollment -Confirm:$False

            } -AsJob

            WaitJob $EnrollmentPromotion

        }

        #If the server was chosen as an online responder, install the role
        if($Global:CAResponder[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)] -eq "Checked") {

        $VMStatusTextBox.AppendText("`r`nPromoting $CAServer to an Online Responder")
    
            $ResponderPromotion = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($CAServer)] -credential $DomainAdminCreds -ScriptBlock {

            Install-AdcsOnlineResponder -Confirm:$False

            } -AsJob

            WaitJob $ResponderPromotion
    
        }

    WaitScript 15

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function CACreation {

$ServerStatusLabel.Text = "Starting Certificate Authority Creation `n`nNote: This Will Take About 5-15 Minutes to Complete"

$VMStatusTextBox.AppendText("`r`n=========================")
$VMStatusTextBox.AppendText("`r`nStarting Certificate Authority Creation")
$VMStatusTextBox.AppendText("`r`n=========================`r`n")

InstallCertificateComponents

WaitScript 15

InstallAllServices

$VMStatusTextBox.AppendText("`r`n=========================")
$VMStatusTextBox.AppendText("`r`nCertificate Authority Creation Complete")
$VMStatusTextBox.AppendText("`r`n=========================`r`n")

}


Function ValidateCertificateCreation {

$CertificateValidationCount = 0
$NoCAName = @()
$NoCAType = @()
$NoParentCA = @()

    foreach($CAServer in $CertificateAuthoritiesListBox.Items) {
    
        if($Global:CANames[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)].Length -lt 1) {
        
        $NoCAName += "`t- $($CAServer)`n"

        }

        if($Global:CATypes[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)].Length -lt 1) {
        
        $NoCAType += "`t- $($CAServer)`n"

        }

        if($Global:CATypes[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)] -match "Subordinate") {

            if($Global:ParentCA[$CertificateAuthoritiesListBox.Items.IndexOf($CAServer)].Length -lt 1) {

            $NoParentCA += "`t- $($CAServer)`n"

            }

        }
    
    $CAValidationCounter++
    }


    if($NoCAName -or $NoCAType -or $NoParentCA) {
    
    [System.Windows.MessageBox]::Show("Error, No Configuraiton Found for the Following CA Servers:`n`nCA Name:`n`n$NoCAName`n`nCA Type:`n`n$NoCAType`n`nParent CA:`n`n$NoParentCA")
    $CertificateBuildoutButton.Enabled = $False
    
    }else{ $CertificateValidationCount++ }


    if($CertificateAuthoritiesListBox.Items.Count -lt 1) {
    
    [System.Windows.MessageBox]::Show("Error in Certificate Authorities ListBox: No Certificate Authorities selected")
    $CertificateBuildoutButton.Enabled = $False

    }else{ $CertificateValidationCount++ }


    if($CertificateValidationCount -eq 2) {
    
    $CertificateBuildoutButton.Enabled = $True
    
    }

}


#################################  DFS BUILDOUT  #################################

Function EnableMakeDFSPrimary {

    if($DFSServersListBox.Items.Count -ge 2) {
                
    $MakeDFSPrimaryButton.Enabled = $True
                
    }

    else {
                
    $MakeDFSPrimaryButton.Enabled = $False
                
    }

    if($DFSServersListBox.Items.Count -eq 1 -and $DFSServersListBox.Items[0] -notmatch [regex]"\*") {
    
    $DFSServersListBox.Items.Add("$($DFSServersListBox.Items[0])*")
    $DFSServersListBox.Items.Remove($DFSServersListBox.Items[0])
    
    }

}


Function AddDFSFolder {

$AddDFSFolderValidation = 0
            
    if($DFSFolderTextBox.Text.Length -lt 1) {

    [System.Windows.MessageBox]::Show("Error in DFS Folder Name TextBox: No folder name input")

    } else { $AddDFSFolderValidation++ }   

    if(!($DropDownDFSRoot.SelectedItem)) {
            
    [System.Windows.MessageBox]::Show("Error in DFS Root Selection DropDown: No DFS root selected")
            
    } else { $AddDFSFolderValidation++ }

    if($DFSTargetTextBox.Text.Length -ge 1){

        if($DFSTargetTextBox.Text -notmatch [regex]'[a-zA-Z][:][\\][a-zA-Z0-9\-_.$\\]{1,}') {

        [System.Windows.MessageBox]::Show("Error in DFS Folder Target TextBox: A folder target was specified, but it either does not meet the length requirements or is not in the correct format")

        } else { $AddDFSFolderValidation++ }

    }

    if(($AddDFSFolderValidation -eq 2 -and $DFSTargetTextBox.Text.Length -lt 1) -or ($AddDFSFolderValidation -eq 3 -and $DFSTargetTextBox.Text -match [regex]'[a-zA-Z][:][\\][a-zA-Z0-9\-_.$\\]{1,}')) {

    $DFSFoldersListBox.Items.Add($DFSFolderTextBox.Text)
     
        foreach($DFSFolder in $DFSFoldersListBox.Items | Sort) {
                
        $DFSFoldersListBox.Items.Remove($DFSFolder)
        $DFSFoldersListBox.Items.Add($DFSFolder)
                
        } 
             
        while($Global:DFSFolders.Count -ne $DFSFoldersListBox.Items.Count) {
              
        $Global:DFSFolders += $Null
        $Global:DFSFolderRoot += $Null
        $Global:DFSFolderTarget += $Null
             
        }

        $Global:DFSFolders.Insert($DFSFoldersListBox.Items.IndexOf($DFSFolderTextBox.Text),$DFSFolderTextBox.Text)
        $Global:DFSFolderRoot.Insert($DFSFoldersListBox.Items.IndexOf($DFSFolderTextBox.Text),$DropDownDFSRoot.Text)
        
        if($AddDFSFolderValidation -eq 3) {
        
        $Global:DFSFolderTarget.Insert($DFSFoldersListBox.Items.IndexOf($DFSFolderTextBox.Text),$DFSTargetTextBox.Text)

        }

        else {
        
        $Global:DFSFolderTarget.Insert($DFSFoldersListBox.Items.IndexOf($DFSFolderTextBox.Text),$Null)
        
        }

    $DFSFolderTextBox.Clear()
    $DropDownDFSRoot.ResetText()
    $DFSTargetTextBox.Clear()

    }

}


Function InstallDFSComponents {

$DomainName = $DomainNameTextBox.Text
$ConnectionPassword = convertto-securestring -AsPlainText -Force -String $LocalPasswordTextBox.Text
$DomainAdminCreds = new-object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

    foreach($DFSServer in $DFSServersListBox.Items){

        if($DFSServer -match [regex]'\*') {
            
        $DFSServer = $DFSServer.Replace("*","")
            
        }

    $VMStatusTextBox.AppendText("`r`nPromoting $DFSServer to a DFS Server")

        $DFSPromotion = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DFSServer)] -credential $DomainAdminCreds -ScriptBlock {

        Install-WindowsFeature FS-FileServer -IncludeManagementTools
        Install-WindowsFeature FS-DFS-Namespace -IncludeManagementTools
        Install-WindowsFeature FS-DFS-Replication -IncludeManagementTools

        } -AsJob

        WaitJob $DFSPromotion

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function CreateNamespaces {

$DomainName = $DomainNameTextBox.Text
$ConnectionPassword = convertto-securestring -AsPlainText -Force -String $LocalPasswordTextBox.Text
$DomainAdminCreds = new-object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

$PrimaryDC = ($DomainControllersListBox.Items | where { $_ -match [regex]"\*" }).ToString().Replace("*","")

    foreach($DFSNamespace in $DFSRootsListBox.Items) {

        $VMStatusTextBox.AppendText("`r`nCreating DFS NameSpace $DFSNamespace")

        foreach($DFSServer in $DFSServersListBox.Items) {
        
            if($DFSServer -match [regex]'\*') {
            
            $DFSServer = $DFSServer.Replace("*","")
            
            }     
            
            $VMStatusTextBox.AppendText("`r`nCreating DFS Namespace Folder '$DFSNamespace' on $DFSServer")

            $FolderCreation = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DFSServer)] -Credential $DomainAdminCreds -ScriptBlock {

            param ($DFSNamespace)
            
            New-Item -ItemType Directory -Path "C:\DFSRoots\" -Name $DFSNamespace -Force
            New-SmbShare -Path "C:\DFSRoots\$DFSNamespace" -Name $DFSNamespace
            Grant-SmbShareAccess -Name "$DFSNamespace" -AccountName "Everyone" -AccessRight Full -Force

            } -ArgumentList $DFSNamespace -AsJob
            
            WaitJob $FolderCreation    

            WaitScript 5

            $RootCreation = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DFSServer)] -credential $DomainAdminCreds -ScriptBlock {

            param ($DFSServer,$DFSNamespace,$DomainName,$DomainAdminCreds,$PrimaryDC)

                Invoke-Command -ComputerName $PrimaryDC -credential $DomainAdminCreds -ScriptBlock {

                param ($DFSServer,$DFSNamespace,$DomainName)

                New-DfsnRoot -Type DomainV2 -TargetPath "\\$DFSServer\$DFSNamespace" -Path "\\$DomainName\$DFSNamespace"

                } -ArgumentList $DFSServer,$DFSNamespace,$DomainName

            } -ArgumentList $DFSServer,$DFSNamespace,$DomainName,$DomainAdminCreds,$PrimaryDC -AsJob

            WaitJob $RootCreation

        }

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function CreateDFSFolders {

#Define necessary connection parameters 
$DomainName = $DomainNameTextBox.Text
$ConnectionPassword = convertto-securestring -AsPlainText -Force -String $LocalPasswordTextBox.Text
$DomainAdminCreds = new-object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

#Define the primary domain controller to execute all the commands on
$PrimaryDC = ($DomainControllersListBox.Items | where { $_ -match [regex]"\*" }).ToString().Replace("*","")

    foreach($DFSFolder in $DFSFoldersListBox.Items){
        
        #If there was a DFS folder target specified, continue with creating that folder and the folder in C:\DFSRoots\<Namespace>
        if($Global:DFSFolderTarget[$Global:DFSFolders.IndexOf($DFSFolder)] -ne $Null) {

            foreach($DFSServer in $DFSServersListBox.Items) {
            
            $DFSPath = "\\$DomainName\$($Global:DFSFolderRoot[$Global:DFSFolders.IndexOf($DFSFolder)])\$DFSFolder"

                if($DFSServer -match [regex]'\*') {
            
                $DFSServer = $DFSServer.Replace("*","")
            
                }

                $DFSFolderCreation = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DFSServer)] -credential $DomainAdminCreds -ScriptBlock {

                param ($DFSFolder,$DFSRoot)

                #Create new DFS folder and share it
                New-Item -ItemType Directory -Path "C:\DFSRoots\$DFSRoot\" -Name "$DFSFolder" -Force
                New-SmbShare -Path "C:\DFSRoots\$DFSRoot\$DFSFolder" -Name "$DFSRoot\$DFSFolder"
                Grant-SmbShareAccess -Name "$DFSRoot\$DFSFolder" -AccountName "Everyone" -AccessRight Full -Force 

                } -ArgumentList $DFSFolder,$Global:DFSFolderRoot[$Global:DFSFolders.IndexOf($DFSFolder)] -AsJob

                WaitJob $DFSFolderCreation

                WaitScript 5

                $VMStatusTextBox.AppendText("`r`nCreating DFS Folder '$DFSFolder' on $DFSServer")

                $FolderTarget = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DFSServer)] -credential $DomainAdminCreds -ScriptBlock {

                param ($DFSPath,$DFSServer,$DFSFolder,$DomainAdminCreds,$PrimaryDC,$OriginalServer)

                    Invoke-Command -ComputerName $PrimaryDC -credential $DomainAdminCreds -ScriptBlock {

                    param ($DFSPath,$DFSServer,$DFSFolder,$OriginalServer)

                        #If this is the primary DFS server, use the DfsnFolder command, otherwise use DfsnFolderTarget
                        if($OriginalServer -match [regex]"\*") {

                        New-DfsnFolder -Path "$DFSPath" -TargetPath "\\$DFSServer\$DFSFolder"
                
                        }

                        else {
                
                        New-DfsnFolderTarget -Path "$DFSPath" -TargetPath "\\$DFSServer\$DFSFolder"
                
                        }

                    } -ArgumentList $DFSPath,$DFSServer,$DFSFolder,$OriginalServer

                } -ArgumentList $DFSPath,$DFSServer,$DFSFolder,$DomainAdminCreds,$PrimaryDC,($DFSServersListBox.Items | where {$_ -match $DFSServer}) -AsJob

                WaitJob $FolderTarget
        
            }

        }

        #Else, just make the new folder in C:\DFSRoots\<Namespace>
        else {

            foreach($DFSServer in $DFSServersListBox.Items) {

                if($DFSServer -match [regex]'\*') {
            
                $DFSServer = $DFSServer.Replace("*","")
            
                }

                $VMStatusTextBox.AppendText("`r`nCreating DFS Folder '$DFSFolder' on $DFSServer")

                $StandaloneFolder = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DFSServer)] -credential $DomainAdminCreds -ScriptBlock {

                param ($DFSFolder,$DFSRoot)  
                
                #Create new DFS folder and share it
                New-Item -ItemType Directory -Path "C:\DFSRoots\$DFSRoot\" -Name $DFSFolder -Force
                New-SmbShare -Path "C:\DFSRoots\$DFSRoot\$DFSFolder" -Name "$DFSFolder"
                Grant-SmbShareAccess -Name "$DFSFolder" -AccountName "Everyone" -AccessRight Full -Force

                } -ArgumentList $DFSFolder,$Global:DFSFolderRoot[$Global:DFSFolders.IndexOf($DFSFolder)] -AsJob

                WaitJob $StandaloneFolder
            
            }

        }

    }

$VMStatusTextBox.AppendText("`r`n")

}


Function CreateReplicationGroups {

$DomainName = $DomainNameTextBox.Text
$ConnectionPassword = convertto-securestring -AsPlainText -Force -String $LocalPasswordTextBox.Text
$DomainAdminCreds = new-object -typename System.Management.Automation.PSCredential -argumentlist "$($DomainName.Remove($DomainName.IndexOf(".")).ToUpper())\Administrator",$ConnectionPassword

$PrimaryDC = ($DomainControllersListBox.Items | where { $_ -match [regex]"\*" }).ToString().Replace("*","")

    foreach($DFSFolder in $DFSFoldersListBox.Items) {

    $ReplGroupName = "$DomainName\$($Global:DFSFolderRoot[$Global:DFSFolders.IndexOf($DFSFolder)])\$DFSFolder"

        foreach($DFSServer in ($DFSServersListBox.Items | where {$_ -match [regex]"\*"})) {
        
        $VMStatusTextBox.AppendText("`r`nCreating Replication Group $ReplGroupName")

            $ReplicationGroup = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($PrimaryDC)] -credential $DomainAdminCreds -ScriptBlock {

            param ($ReplGroupName,$DFSFolder)

            New-DfsReplicationGroup -GroupName "$ReplGroupName" | New-DfsReplicatedFolder -FolderName "$DFSFolder" -DfsnPath "\\$ReplGroupName"

            } -ArgumentList $ReplGroupName,$DFSFolder -AsJob

            WaitJob $ReplicationGroup
       
        }

        WaitScript 5

        $VMStatusTextBox.AppendText("`r`n Adding Members and Connections to $ReplGroupName")

        foreach($DFSServer in $DFSServersListBox.Items) {

        $PrimaryDFSServer = ($DFSServersListBox.Items | where {$_ -match [regex]"\*"}).Replace("*","")

            if($DFSServer -match [regex]'\*') {
            
            $DFSServer = $DFSServer.Replace("*","")
            
            }

            $ReplGroupProperties = Invoke-Command -ComputerName $Global:IPAddresses[($Global:AllCreatedServers | sort).IndexOf($DFSServer)] -credential $DomainAdminCreds -ScriptBlock {

            param ($ReplGroupName,$PrimaryDFSServer,$DFSServer,$DFSFolder,$DFSFolderTarget,$DFSFolderRoot,$DomainAdminCreds,$PrimaryDC)

                if($DFSFolderTarget -eq $Null) {
            
                $DFSFolderTarget = "C:\DFSRoots\$DFSFolderRoot\$DFSFolder"
            
                }

                if(!(Test-Path $DFSFolderTarget -ErrorAction SilentlyContinue)) {
                    
                New-Item -Path $DFSFolderTarget.Remove($DFSFolderTarget.LastIndexOf("\")+1) -Name $DFSFolderTarget.Substring($DFSFolderTarget.LastIndexOf("\")+1) -ItemType Directory -Force
                New-SmbShare -Path "$DFSFolderTarget" -Name $DFSFolder
                Grant-SmbShareAccess -Name "$DFSFolder" -AccountName "Everyone" -AccessRight Full -Force
                    
                }

                else {
                    
                New-SmbShare -Path "$DFSFolderTarget" -Name $DFSFolder
                Grant-SmbShareAccess -Name "$DFSFolder" -AccountName "Everyone" -AccessRight Full -Force
                    
                }

                WaitScript 5

                Invoke-Command -ComputerName $PrimaryDC -Credential $DomainAdminCreds -ScriptBlock {

                param ($ReplGroupName,$PrimaryDFSServer,$DFSServer,$DFSFolder,$DFSFolderTarget,$DFSFolderRoot)

                Add-DfsrMember -GroupName "$ReplGroupName" -ComputerName "$DFSServer"

                Start-Sleep 3

                    Get-DfsrMember | % { 
            
                        if($_.ComputerName -match $PrimaryDFSServer) {

                        Set-DfsrMembership -GroupName "$ReplGroupName" -FolderName "$DFSFolder" -ContentPath "$DFSFolderTarget" -PrimaryMember $True -ComputerName $_.ComputerName -Force

                        }

                        else {
            
                        Set-DfsrMembership -GroupName "$ReplGroupName" -FolderName "$DFSFolder" -ContentPath "$DFSFolderTarget" -ComputerName $_.ComputerName -Force

                        }
            
                    }

                Add-DfsrConnection -GroupName "$ReplGroupName" -SourceComputerName "$PrimaryDFSServer" -DestinationComputerName "$DFSServer" -ErrorAction SilentlyContinue

                } -ArgumentList $ReplGroupName,$PrimaryDFSServer,$DFSServer,$DFSFolder,$DFSFolderTarget,$DFSFolderRoot

            } -ArgumentList $ReplGroupName,$PrimaryDFSServer,$DFSServer,$DFSFolder,$Global:DFSFolderTarget[$Global:DFSFolders.IndexOf($DFSFolder)],$Global:DFSFolderRoot[$Global:DFSFolders.IndexOf($DFSFolder)],$DomainAdminCreds,$PrimaryDC -AsJob

            WaitJob $ReplGroupProperties

        }
    
    }

$VMStatusTextBox.AppendText("`r`n")

}


Function DFSCreation {

$ServerStatusLabel.Text = "Starting Certificate Authority Creation `n`nNote: This Will Take About 2-10 Minutes to Complete"

$VMStatusTextBox.AppendText("`r`n=========================")
$VMStatusTextBox.AppendText("`r`nStarting DFS Creation")
$VMStatusTextBox.AppendText("`r`n=========================`r`n")

InstallDFSComponents

WaitScript 10

CreateNamespaces

WaitScript 10

CreateDFSFolders

WaitScript 10

CreateReplicationGroups

$VMStatusTextBox.AppendText("`r`n=========================")
$VMStatusTextBox.AppendText("`r`nDFS Creation Complete")
$VMStatusTextBox.AppendText("`r`n=========================`r`n")

}


Function ValidateDFSCreation {

$DFSValidationCount = 0

    if($DFSServersListBox.Items.Count -lt 1) {

    [System.Windows.MessageBox]::Show("Error in DFS Servers ListBox: No DFS Servers selected")
    $DFSBuildoutButton.Enabled = $False

    }else{ $DFSValidationCount++ }

    if($DFSRootsListBox.Items.Count -lt 1) {

    [System.Windows.MessageBox]::Show("Error in DFS Roots ListBox: No DFS Roots Specified")
    $DFSBuildoutButton.Enabled = $False

    }else{ $DFSValidationCount++ }

    if($DFSFoldersListBox.Items.Count -lt 1) {

    [System.Windows.MessageBox]::Show("Error in DFS Folders ListBox: No DFS Folder Specified")
    $DFSBuildoutButton.Enabled = $False

    }else{ $DFSValidationCount++ }

    if($DFSValidationCount -eq 3) {
    
    $DFSBuildoutButton.Enabled = $True
    
    }

}


#################################  COMPLETE COMPONENT BUILDOUT  #################################

Function CompleteBuildout {

$VMStatusTextBox.AppendText("=========================")
$VMStatusTextBox.AppendText("`r`nStarting Complete Buildout")
$VMStatusTextBox.AppendText("`r`n=========================`r`n")

$WinRMPreviouslyStarted = $False
$PreviousTrustedHostValue = $Null

#Start WinRM service for remote powershell if not already started
    if((Get-Service WinRM).Status -eq "Stopped") {

    $VMStatusTextBox.AppendText("`r`nStarting the WinRM Service")
    Get-Service WinRM | Start-Service -Confirm:$False

    }

    elseif((Get-Service WinRM).Status -eq "Running") {
    
    $WinRMPreviouslyStarted = $True
    
    }

#Change trusted hosts file to allow for remote PowerShell Using IP addresses
    if((Get-Item wsman:\localhost\Client\TrustedHosts).Value -notmatch [regex]"\*") {

    $PreviousTrustedHostValue = (Get-Item wsman:\localhost\Client\TrustedHosts).Value
    $VMStatusTextBox.AppendText("`r`nModifying the Trusted Hosts File to Allow for Remote PoSH")
    Start-Process PowerShell -Verb RunAs -ArgumentList {Set-Item wsman:\localhost\Client\TrustedHosts -value * -Confirm:$False -Force} -WindowStyle Hidden

    }

$VMStatusTextBox.AppendText("`r`n")
$CheckStateListView.Hide()
$VMStatusTextBox.Show()

DomainCreation

    if($Global:OULocation.Count -ge 1 -or $Global:GroupName.Count -ge 1 -or $Global:UserLoginName.Count -ge 1) {

    UGOCreation
    
    }

    if($CertificateServicesCheckbox.CheckState -eq "Checked") {
            
    CACreation
            
    }

    if($DFSCheckbox.CheckState -eq "Checked") {
            
    DFSCreation
            
    }

#Reset Trusted Hosts file to default of Null, or, if previously conigured, change it back to that configuration 
    if($PreviousTrustedHostValue -ne $Null -or $PreviousTrustedHostValue -notmatch [regex]"\*") {

    $VMStatusTextBox.AppendText("`r`nReverting the Trusted Hosts File Value")
    Start-Process PowerShell -Verb RunAs -ArgumentList ("`$PreviousTrustedHostValue = '$PreviousTrustedHostValue';" + {Set-Item wsman:\localhost\Client\TrustedHosts -value $PreviousTrustedHostValue -Confirm:$False -Force}) -WindowStyle Hidden

    }
    
    else {

    $VMStatusTextBox.AppendText("`r`nReverting the Trusted Hosts File Value to Null")
    Start-Process PowerShell -Verb RunAs -ArgumentList {Clear-Item "wsman:\localhost\Client\TrustedHosts" -Force} -WindowStyle Hidden
 
    }

#Stop WinRM service if it wasn't already started
    if($WinRMPreviouslyStarted -eq $False) {
    
    $VMStatusTextBox.AppendText("`r`nStopping the WinRM Service")
    Get-Service WinRM | Stop-Service -Confirm:$False

    }

    DisconnectXenHost

    $ServerStatusLabel.Text = "- Removing Jobs - `n`nThis Will Take a few Minutes"

    Get-Job | Stop-Job

    Get-Job | Remove-Job

$ServerStatusLabel.Text = "Click 'Complete' to Exit"

}


#################################  FORMS  #################################

Function ISOConstructionForm {

#################################  ISO CONSTRUCTION FORM  #################################

$ISOConstructionForm = New-Object system.Windows.Forms.Form
$ISOConstructionForm.Text = "AXL"
$ISOConstructionForm.ClientSize = new-object System.Drawing.Size(330,545)
$ISOConstructionForm.FormBorderStyle = 'Fixed3D'
$ISOConstructionForm.MaximizeBox = $False
$ISOConstructionForm.StartPosition = 'CenterScreen'



        #################################  CHECK BOXES  #################################

        $InstallXenToolsCheckbox = New-Object System.Windows.Forms.Checkbox 
        $InstallXenToolsCheckbox.Location = New-Object System.Drawing.Size(215,255) 
        $InstallXenToolsCheckbox.AutoSize = $True
            $InstallXenToolsCheckbox.Add_CheckStateChanged({

                if($InstallXenToolsCheckbox.CheckState -eq 'Checked') {
            
                $XenToolsPathTextBox.Enabled = $True
                $BrowseXenToolsButton.Enabled = $True
                AutoPopulateTextBoxes
            
                }
            
                else {
            
                $XenToolsPathTextBox.Enabled = $False
                $BrowseXenToolsButton.Enabled = $False
                $XenToolsPathTextBox.Clear()
            
                }


            })
        $ISOConstructionForm.Controls.Add($InstallXenToolsCheckbox)


        $Windows10Checkbox = New-Object System.Windows.Forms.Checkbox 
        $Windows10Checkbox.Location = New-Object System.Drawing.Size(180,465)
        $Windows10Checkbox.Text = "Windows 10" 
        $Windows10Checkbox.AutoSize = $True
            $Windows10Checkbox.Add_CheckStateChanged({

                if($Windows10Checkbox.CheckState -eq 'Checked') {

                $DropDownEditionSelection.Enabled = $True
                $DropDownEditionSelection.Items.Clear()
                $DropDownEditionSelection.ResetText()

                    if($Server2016Checkbox.CheckState -eq 'Checked'){
                
                    $Server2016Checkbox.Checked = $False
                
                    }

                AddEditions
            
                }
                elseif($Server2016Checkbox.CheckState -eq 'Unchecked' -and $Windows10Checkbox.CheckState -eq 'Unchecked'){
                
                $DropDownEditionSelection.Enabled = $False

                }

            })
        $ISOConstructionForm.Controls.Add($Windows10Checkbox)


        $Server2016Checkbox = New-Object System.Windows.Forms.Checkbox 
        $Server2016Checkbox.Location = New-Object System.Drawing.Size(180,485) 
        $Server2016Checkbox.Text = "Server 2016"
        $Server2016Checkbox.AutoSize = $True
            $Server2016Checkbox.Add_CheckStateChanged({

                if($Server2016Checkbox.CheckState -eq 'Checked') {

                $DropDownEditionSelection.Enabled = $True
                $DropDownEditionSelection.Items.Clear()
                $DropDownEditionSelection.ResetText()

                    if($Windows10Checkbox.CheckState -eq 'Checked'){
                
                    $Windows10Checkbox.Checked = $False
                
                    }
            
                AddEditions

                }
                elseif($Server2016Checkbox.CheckState -eq 'Unchecked' -and $Windows10Checkbox.CheckState -eq 'Unchecked'){
                
                $DropDownEditionSelection.Enabled = $False

                }

            })
        $ISOConstructionForm.Controls.Add($Server2016Checkbox)



        #################################  LABELS  #################################

        $ISOPathLabel = New-Object System.Windows.Forms.Label
        $ISOPathLabel.Text = "Input ISO Location:"
        $ISOPathLabel.AutoSize = $True
        $ISOPathLabel.Location = new-object System.Drawing.Size(10,5)
        $ISOConstructionForm.Controls.Add($ISOPathLabel)


        $TargetFolderLabel = New-Object System.Windows.Forms.Label
        $TargetFolderLabel.Text = "Input Target Folder Location:"
        $TargetFolderLabel.AutoSize = $True
        $TargetFolderLabel.Location = new-object System.Drawing.Size(10,55)
        $ISOConstructionForm.Controls.Add($TargetFolderLabel)


        $AutounattendPathLabel = New-Object System.Windows.Forms.Label
        $AutounattendPathLabel.Text = "Input Autounattend.xml File Location:"
        $AutounattendPathLabel.AutoSize = $True
        $AutounattendPathLabel.Location = new-object System.Drawing.Size(10,105)
        $ISOConstructionForm.Controls.Add($AutounattendPathLabel)


        $BootFilePathLabel = New-Object System.Windows.Forms.Label
        $BootFilePathLabel.Text = "Input Boot File Location (etfsboot.com):"
        $BootFilePathLabel.AutoSize = $True
        $BootFilePathLabel.Location = new-object System.Drawing.Size(10,155)
        $ISOConstructionForm.Controls.Add($BootFilePathLabel)


        $ISOToolPathLabel = New-Object System.Windows.Forms.Label
        $ISOToolPathLabel.Text = "Input Path to ISO Creation Tool (oscdimg.exe):"
        $ISOToolPathLabel.AutoSize = $True
        $ISOToolPathLabel.Location = new-object System.Drawing.Size(10,205)
        $ISOConstructionForm.Controls.Add($ISOToolPathLabel)


        $XenToolsPathLabel = New-Object System.Windows.Forms.Label
        $XenToolsPathLabel.Text = "Input path to Xen Tools contents:                    Install Xen Tools? "
        $XenToolsPathLabel.AutoSize = $True
        $XenToolsPathLabel.Location = new-object System.Drawing.Size(10,255)
        $ISOConstructionForm.Controls.Add($XenToolsPathLabel)


        $AdminNameLabel = New-Object System.Windows.Forms.Label
        $AdminNameLabel.Text = "Admin Name:"
        $AdminNameLabel.AutoSize = $True
        $AdminNameLabel.Location = new-object System.Drawing.Size(10,355)
        $ISOConstructionForm.Controls.Add($AdminNameLabel)


        $AdminPasswordLabel = New-Object System.Windows.Forms.Label
        $AdminPasswordLabel.Text = "Admin Password:"
        $AdminPasswordLabel.AutoSize = $True
        $AdminPasswordLabel.Location = new-object System.Drawing.Size(10,405)
        $ISOConstructionForm.Controls.Add($AdminPasswordLabel)


        $ProductKeyLabel = New-Object System.Windows.Forms.Label
        $ProductKeyLabel.Text = "Input Product Key:"
        $ProductKeyLabel.AutoSize = $True
        $ProductKeyLabel.Location = new-object System.Drawing.Size(10,305)
        $ISOConstructionForm.Controls.Add($ProductKeyLabel)


        $NewISONameLabel = New-Object System.Windows.Forms.Label
        $NewISONameLabel.Text = "New ISO File Name:"
        $NewISONameLabel.AutoSize = $True
        $NewISONameLabel.Location = new-object System.Drawing.Size(125,355)
        $ISOConstructionForm.Controls.Add($NewISONameLabel)


        $EditionLabel = New-Object System.Windows.Forms.Label
        $EditionLabel.Text = "Select Edition:"
        $EditionLabel.AutoSize = $True
        $EditionLabel.Location = new-object System.Drawing.Size(10,455)
        $ISOConstructionForm.Controls.Add($EditionLabel)


        $TimeZoneLabel = New-Object System.Windows.Forms.Label
        $TimeZoneLabel.Text = "Select TimeZone:"
        $TimeZoneLabel.AutoSize = $True
        $TimeZoneLabel.Location = new-object System.Drawing.Size(125,405)
        $ISOConstructionForm.Controls.Add($TimeZoneLabel)


        $ISOCopyProgressLabel = New-Object System.Windows.Forms.Label
        $ISOCopyProgressLabel.Size = new-object System.Drawing.Size(310,30)
        $ISOCopyProgressLabel.Location = new-object System.Drawing.Size(10,510)
        $ISOCopyProgressLabel.Visible = $False
        $ISOConstructionForm.Controls.Add($ISOCopyProgressLabel)



        #################################  BUTTONS  #################################


        $BrowseISOButton = new-object System.Windows.Forms.Button
        $BrowseISOButton.Location = new-object System.Drawing.Size(260,25)
        $BrowseISOButton.Size = new-object System.Drawing.Size(60,20)
        $BrowseISOButton.Text = "Browse..."
            $BrowseISOButton.Add_Click({

            $ISOPathTextBox.Text = BrowseFiles

            })
        $ISOConstructionForm.Controls.Add($BrowseISOButton) 
        
          
        $BrowseTargetFolderButton = new-object System.Windows.Forms.Button
        $BrowseTargetFolderButton.Location = new-object System.Drawing.Size(260,75)
        $BrowseTargetFolderButton.Size = new-object System.Drawing.Size(60,20)
        $BrowseTargetFolderButton.Text = "Browse..."
            $BrowseTargetFolderButton.Add_Click({

            $TargetFolderTextBox.Text = BrowseFolders

            })
        $ISOConstructionForm.Controls.Add($BrowseTargetFolderButton)
        

        $BrowseAutounattendButton = new-object System.Windows.Forms.Button
        $BrowseAutounattendButton.Location = new-object System.Drawing.Size(260,125)
        $BrowseAutounattendButton.Size = new-object System.Drawing.Size(60,20)
        $BrowseAutounattendButton.Text = "Browse..."
            $BrowseAutounattendButton.Add_Click({

            $AutounattendPathTextBox.Text = BrowseFiles

            })
        $ISOConstructionForm.Controls.Add($BrowseAutounattendButton)  
        
              
        $BrowseBootFileButton = new-object System.Windows.Forms.Button
        $BrowseBootFileButton.Location = new-object System.Drawing.Size(260,175)
        $BrowseBootFileButton.Size = new-object System.Drawing.Size(60,20)
        $BrowseBootFileButton.Text = "Browse..."
            $BrowseBootFileButton.Add_Click({

            $BootFilePathTextBox.Text = BrowseFiles

            })
        $ISOConstructionForm.Controls.Add($BrowseBootFileButton)
        

        $BrowseISOToolButton = new-object System.Windows.Forms.Button
        $BrowseISOToolButton.Location = new-object System.Drawing.Size(260,225)
        $BrowseISOToolButton.Size = new-object System.Drawing.Size(60,20)
        $BrowseISOToolButton.Text = "Browse..."
            $BrowseISOToolButton.Add_Click({

            $ISOToolPathTextBox.Text = BrowseFiles

            })
        $ISOConstructionForm.Controls.Add($BrowseISOToolButton)


        $BrowseXenToolsButton = new-object System.Windows.Forms.Button
        $BrowseXenToolsButton.Location = new-object System.Drawing.Size(260,275)
        $BrowseXenToolsButton.Size = new-object System.Drawing.Size(60,20)
        $BrowseXenToolsButton.Text = "Browse..."
        $BrowseXenToolsButton.Enabled = $False
            $BrowseXenToolsButton.Add_Click({

            $XenToolsPathTextBox.Text = BrowseFolders

            })
        $ISOConstructionForm.Controls.Add($BrowseXenToolsButton)


        $CreateISOButton = new-object System.Windows.Forms.Button
        $CreateISOButton.Location = new-object System.Drawing.Size(10,515)
        $CreateISOButton.Size = new-object System.Drawing.Size(90,20)
        $CreateISOButton.Text = "Create"
        $CreateISOButton.Enabled = $False
            $CreateISOButton.Add_Click({
        
            $ValidateISOCreationButton.Visible = $False
            $CopyFileProgressBar.Visible = $True
            $ISOConstructionForm.ClientSize = new-object System.Drawing.Size(330,580)
            $CreateISOButton.Visible = $False
            $ExitISOCreationButton.Visible = $False
            CopyFiles
            $CopyFileProgressBar.Visible = $False
            $ISOConstructionForm.ClientSize = new-object System.Drawing.Size(330,560)
            $ISOCopyProgressLabel.Size = new-object System.Drawing.Size(310,40)
            BuildISO
            RemoveFolderForm
            CreateAdditionalISOsForm

            })
        $ISOConstructionForm.Controls.Add($CreateISOButton)


        $ExitISOCreationButton = new-object System.Windows.Forms.Button
        $ExitISOCreationButton.Location = new-object System.Drawing.Size(230,515)
        $ExitISOCreationButton.Size = new-object System.Drawing.Size(90,20)
        $ExitISOCreationButton.Text = "Exit"
            $ExitISOCreationButton.Add_Click({

            $ISOConstructionForm.Close()

            })
        $ISOConstructionForm.Controls.Add($ExitISOCreationButton)


        $ValidateISOCreationButton = new-object System.Windows.Forms.Button
        $ValidateISOCreationButton.Location = new-object System.Drawing.Size(120,515)
        $ValidateISOCreationButton.Size = new-object System.Drawing.Size(90,20)
        $ValidateISOCreationButton.Text = "Validate"
            $ValidateISOCreationButton.Add_Click({

            CheckISOCompletionState

            })
        $ISOConstructionForm.Controls.Add($ValidateISOCreationButton)



        #################################  TEXT BOXES  #################################

        $ISOPathTextBox = new-object System.Windows.Forms.TextBox
        $ISOPathTextBox.Location = new-object System.Drawing.Size(10,25)
        $ISOPathTextBox.Size = new-object System.Drawing.Size(240,20)
            $ISOPathTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($ISOPathTextBox)
        

        $TargetFolderTextBox = new-object System.Windows.Forms.TextBox
        $TargetFolderTextBox.Location = new-object System.Drawing.Size(10,75)
        $TargetFolderTextBox.Size = new-object System.Drawing.Size(240,20)
            $TargetFolderTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($TargetFolderTextBox)


        $AutounattendPathTextBox = new-object System.Windows.Forms.TextBox
        $AutounattendPathTextBox.Location = new-object System.Drawing.Size(10,125)
        $AutounattendPathTextBox.Size = new-object System.Drawing.Size(240,20)
            $AutounattendPathTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($AutounattendPathTextBox)


        $BootFilePathTextBox = new-object System.Windows.Forms.TextBox
        $BootFilePathTextBox.Location = new-object System.Drawing.Size(10,175)
        $BootFilePathTextBox.Size = new-object System.Drawing.Size(240,20)
            $BootFilePathTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($BootFilePathTextBox)


        $ISOToolPathTextBox = new-object System.Windows.Forms.TextBox
        $ISOToolPathTextBox.Location = new-object System.Drawing.Size(10,225)
        $ISOToolPathTextBox.Size = new-object System.Drawing.Size(240,20)
            $ISOToolPathTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($ISOToolPathTextBox)


        $XenToolsPathTextBox = new-object System.Windows.Forms.TextBox
        $XenToolsPathTextBox.Location = new-object System.Drawing.Size(10,275)
        $XenToolsPathTextBox.Size = new-object System.Drawing.Size(240,20)
        $XenToolsPathTextBox.Enabled = $False
            $XenToolsPathTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($XenToolsPathTextBox)


        $ProductKeyTextBox = new-object System.Windows.Forms.TextBox
        $ProductKeyTextBox.Location = new-object System.Drawing.Size(10,325)
        $ProductKeyTextBox.Size = new-object System.Drawing.Size(310,20)
            $ProductKeyTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($ProductKeyTextBox)


        $AdminNameTextBox = new-object System.Windows.Forms.TextBox
        $AdminNameTextBox.Location = new-object System.Drawing.Size(10,375)
        $AdminNameTextBox.Size = new-object System.Drawing.Size(105,20)
            $AdminNameTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($AdminNameTextBox)


        $AdminPasswordTextBox = new-object System.Windows.Forms.TextBox
        $AdminPasswordTextBox.Location = new-object System.Drawing.Size(10,425)
        $AdminPasswordTextBox.Size = new-object System.Drawing.Size(105,20)
        $AdminPasswordTextBox.PasswordChar = '*'
            $AdminPasswordTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($AdminPasswordTextBox)


        $NewISONameTextBox = new-object System.Windows.Forms.TextBox
        $NewISONameTextBox.Location = new-object System.Drawing.Size(125,375)
        $NewISONameTextBox.Size = new-object System.Drawing.Size(195,20)
            $NewISONameTextBox.Add_TextChanged({
        
            $CreateISOButton.Enabled = $False
        
            })
        $ISOConstructionForm.Controls.Add($NewISONameTextBox)

        AutoPopulateTextBoxes



        #################################  DROP DOWN BOXES  #################################

        $DropDownEditionSelection = new-object System.Windows.Forms.ComboBox
        $DropDownEditionSelection.Location = new-object System.Drawing.Size(10,475)
        $DropDownEditionSelection.Size = new-object System.Drawing.Size(160,20)
        $DropDownEditionSelection.Enabled = $False
            $DropDownEditionSelection.Add_SelectedIndexChanged({
            
            $CreateISOButton.Enabled = $False

            })
        $ISOConstructionForm.Controls.Add($DropDownEditionSelection)


        $DropDownTimeZones = new-object System.Windows.Forms.ComboBox
        $DropDownTimeZones.Location = new-object System.Drawing.Size(125,425)
        $DropDownTimeZones.Size = new-object System.Drawing.Size(195,20)
            $DropDownTimeZones.Add_SelectedIndexChanged({
            
            $CreateISOButton.Enabled = $False

            })
        $ISOConstructionForm.Controls.Add($DropDownTimeZones)
        
        AddTimeZones



        #################################  PROGRESS BAR  #################################

        $CopyFileProgressBar = New-Object System.Windows.Forms.ProgressBar
        $CopyFileProgressBar.Minimum = 0
        $CopyFileProgressBar.Location = new-object System.Drawing.Size(10,540)
        $CopyFileProgressBar.Size = new-object System.Drawing.Size(300,30)
        $CopyFileProgressBar.Visible = $False
        $CopyFileProgressBar.Step = 1
        $ISOConstructionForm.Controls.Add($CopyFileProgressBar)



        #################################  Tool Tips  #################################
                	
        $UsernameToolTip = New-Object System.Windows.Forms.ToolTip
        $UsernameToolTip.SetToolTip($AdminNameTextBox, "This is the username that will be used to sign into the machine and run the first logon `ncommands as well as run remote commands for installing additional components if `ndesired. It is highly recommended this be the same across all ISOs created.")
        $UsernameToolTip.AutoPopDelay = 10000
        $UsernameToolTip.InitialDelay = 100
        $UsernameToolTip.ToolTipTitle = "Username Help"
        $UsernameToolTip.ToolTipIcon = "Info"
        $UsernameToolTip.IsBalloon = $True


        $PasswordToolTip = New-Object System.Windows.Forms.ToolTip
        $PasswordToolTip.SetToolTip($AdminPasswordTextBox, "This is the password that will be used to sign into the machine and run the first logon `ncommands as well as run remote commands for installing additional components if `ndesired; this does not need to be a complex password. It is highly recommended this `nbe the same across all ISOs created.")
        $PasswordToolTip.AutoPopDelay = 10000
        $PasswordToolTip.InitialDelay = 100
        $PasswordToolTip.ToolTipTitle = "Password Help"
        $PasswordToolTip.ToolTipIcon = "Info"
        $PasswordToolTip.IsBalloon = $True


$ISOConstructionForm.ShowDialog()

#################################  END FORM  #################################

}


Function CreateAdditionalISOsForm {

#################################  CREATE ADDITIONAL ISOs FORM  #################################

$CreateAdditionalISOsForm = New-Object system.Windows.Forms.Form
$CreateAdditionalISOsForm.Text = "AXL"
$CreateAdditionalISOsForm.ClientSize = new-object System.Drawing.Size(220,60)
$CreateAdditionalISOsForm.FormBorderStyle = 'Fixed3D'
$CreateAdditionalISOsForm.MaximizeBox = $False
$CreateAdditionalISOsForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $MoreVMsLabel = New-Object System.Windows.Forms.Label
        $MoreVMsLabel.Text = "Would you like to create another ISO?"
        $MoreVMsLabel.AutoSize = $True
        $MoreVMsLabel.Location = new-object System.Drawing.Size(15,5)
        $CreateAdditionalISOsForm.Controls.Add($MoreVMsLabel)



        #################################  BUTTONS  #################################


        $MoreVMsYesButton = new-object System.Windows.Forms.Button
        $MoreVMsYesButton.Location = new-object System.Drawing.Size(10,30)
        $MoreVMsYesButton.Size = new-object System.Drawing.Size(90,20)
        $MoreVMsYesButton.Text = "Yes"
            $MoreVMsYesButton.Add_Click({

            $CopyFileProgressBar.Value = 0
            $CreateAdditionalISOsForm.Close()
            $ISOCopyProgressLabel.Visible = $False
            $ExitISOCreationButton.Visible = $True
            $CopyFileProgressBar.Visible = $False
            $CreateISOButton.Visible = $True
            $ValidateISOCreationButton.Visible = $True
            $ISOConstructionForm.ClientSize = new-object System.Drawing.Size(330,550)
            $ISOPathTextBox.Clear()
            $TargetFolderTextBox.Clear()
            $AutounattendPathTextBox.Clear()
            $DropDownEditionSelection.Items.Clear()
            $DropDownEditionSelection.ResetText()
            $DropDownEditionSelection.Enabled = $False
            $DropDownTimeZones.ResetText()
            $ProductKeyTextBox.Clear()
            $AdminNameTextBox.Clear()
            $AdminPasswordTextBox.Clear()
            $NewISONameTextBox.Clear()

                if($InstallXenToolsCheckbox.CheckState -eq 'Checked'){
            
                $InstallXenToolsCheckbox.Checked = $False
            
                }

                if($Windows10Checkbox.CheckState -eq 'Checked') {

                $Windows10Checkbox.Checked = $False

                }

                if($Server2016Checkbox.CheckState -eq 'Checked') {

                $Server2016Checkbox.Checked = $False

                }

            })
        $CreateAdditionalISOsForm.Controls.Add($MoreVMsYesButton) 


        $MoreVMsNoButton = new-object System.Windows.Forms.Button
        $MoreVMsNoButton.Location = new-object System.Drawing.Size(120,30)
        $MoreVMsNoButton.Size = new-object System.Drawing.Size(90,20)
        $MoreVMsNoButton.Text = "No"
            $MoreVMsNoButton.Add_Click({

            $CreateAdditionalISOsForm.Hide()

            CreateVMsForm

            })
        $CreateAdditionalISOsForm.Controls.Add($MoreVMsNoButton) 


$CreateAdditionalISOsForm.ShowDialog()

#################################  END FORM  #################################

}


Function RemoveFolderForm {

#################################  REMOVE FOLDER FORM  #################################

$RemoveFolderForm = New-Object system.Windows.Forms.Form
$RemoveFolderForm.Text = "AXL"
$RemoveFolderForm.ClientSize = new-object System.Drawing.Size(240,80)
$RemoveFolderForm.FormBorderStyle = 'Fixed3D'
$RemoveFolderForm.MaximizeBox = $False
$RemoveFolderForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $RemoveFolderLabel = New-Object System.Windows.Forms.Label
        $RemoveFolderLabel.Text = "Would you like to remove the $($TargetFolderTextBox.Text) folder?"
        $RemoveFolderLabel.Size = new-object System.Drawing.Size(200,40)
        $RemoveFolderLabel.Location = new-object System.Drawing.Size(20,5)
        $RemoveFolderForm.Controls.Add($RemoveFolderLabel)



        #################################  BUTTONS  #################################


        $RemoveFolderYesButton = new-object System.Windows.Forms.Button
        $RemoveFolderYesButton.Location = new-object System.Drawing.Size(10,50)
        $RemoveFolderYesButton.Size = new-object System.Drawing.Size(100,20)
        $RemoveFolderYesButton.Text = "Yes"
            $RemoveFolderYesButton.Add_Click({
            
                Try {

                Remove-Item $TargetFolderTextBox.Text -Force -Recurse
                Remove-Item $TargetFolderTextBox.Text -Force

                }

                Catch {
            
                [System.Windows.MessageBox]::Show("An error occured: Could not remove the folder - $($TargetFolderTextBox.Text)")
            
                }
            
                if(!(Get-ChildItem $TargetFolderTextBox.Text -ErrorAction SilentlyContinue)) {
            
                [System.Windows.MessageBox]::Show("The $($TargetFolderTextBox.Text) folder was successfully removed!")
            
                }  
                
                else {
                
                [System.Windows.MessageBox]::Show("The $($TargetFolderTextBox.Text) folder was not removed!")
                
                }   

            $RemoveFolderForm.Hide()

            })
        $RemoveFolderForm.Controls.Add($RemoveFolderYesButton) 


        $RemoveFolderNoButton = new-object System.Windows.Forms.Button
        $RemoveFolderNoButton.Location = new-object System.Drawing.Size(130,50)
        $RemoveFolderNoButton.Size = new-object System.Drawing.Size(100,20)
        $RemoveFolderNoButton.Text = "No"
            $RemoveFolderNoButton.Add_Click({

            $RemoveFolderForm.Hide()

            })
        $RemoveFolderForm.Controls.Add($RemoveFolderNoButton) 


$RemoveFolderForm.ShowDialog()

#################################  END FORM  #################################

}


Function CreateVMsForm {

#################################  CREATE VMS FORM  #################################

$CreateVMsForm = New-Object system.Windows.Forms.Form
$CreateVMsForm.Text = "AXL"
$CreateVMsForm.ClientSize = new-object System.Drawing.Size(220,60)
$CreateVMsForm.FormBorderStyle = 'Fixed3D'
$CreateVMsForm.MaximizeBox = $False
$CreateVMsForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $MoreVMsLabel = New-Object System.Windows.Forms.Label
        $MoreVMsLabel.Text = "Would you like to create VMs?"
        $MoreVMsLabel.AutoSize = $True
        $MoreVMsLabel.Location = new-object System.Drawing.Size(35,5)
        $CreateVMsForm.Controls.Add($MoreVMsLabel)



        #################################  BUTTONS  #################################


        $MoreVMsYesButton = new-object System.Windows.Forms.Button
        $MoreVMsYesButton.Location = new-object System.Drawing.Size(10,30)
        $MoreVMsYesButton.Size = new-object System.Drawing.Size(90,20)
        $MoreVMsYesButton.Text = "Yes"
            $MoreVMsYesButton.Add_Click({

            DisconnectXenHost 
            $CreateVMsForm.Hide()
            $ISOConstructionForm.Hide()

            VMBuildoutForm

            })
        $CreateVMsForm.Controls.Add($MoreVMsYesButton) 


        $MoreVMsNoButton = new-object System.Windows.Forms.Button
        $MoreVMsNoButton.Location = new-object System.Drawing.Size(120,30)
        $MoreVMsNoButton.Size = new-object System.Drawing.Size(90,20)
        $MoreVMsNoButton.Text = "No"
            $MoreVMsNoButton.Add_Click({

            $CreateVMsForm.Close()
            $ISOConstructionForm.Close()

            })
        $CreateVMsForm.Controls.Add($MoreVMsNoButton) 


$CreateVMsForm.ShowDialog()

#################################  END FORM  #################################

}


Function VMBuildoutForm {

#################################  VM BUILDOUT FORM  #################################

$VMBuildoutForm = New-Object system.Windows.Forms.Form
$VMBuildoutForm.Text = "AXL"
$VMBuildoutForm.ClientSize = new-object System.Drawing.Size(300,465)
$VMBuildoutForm.FormBorderStyle = 'Fixed3D'
$VMBuildoutForm.MaximizeBox = $False
$VMBuildoutForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $SelectStorageLabel = New-Object System.Windows.Forms.Label
        $SelectStorageLabel.Text = "Select Storage Location"
        $SelectStorageLabel.AutoSize = $True
        $SelectStorageLabel.Location = new-object System.Drawing.Size(10,100)
        $VMBuildoutForm.Controls.Add($SelectStorageLabel)


        $TemplateLabel = New-Object System.Windows.Forms.Label
        $TemplateLabel.Text = "Select a Template"
        $TemplateLabel.AutoSize = $True
        $TemplateLabel.Location = new-object System.Drawing.Size(10,280)
        $VMBuildoutForm.Controls.Add($TemplateLabel)


        $XenHostIPLabel = New-Object System.Windows.Forms.Label
        $XenHostIPLabel.Text = "XenServer IP Address:"
        $XenHostIPLabel.AutoSize = $True
        $XenHostIPLabel.Location = new-object System.Drawing.Size(10,5)
        $VMBuildoutForm.Controls.Add($XenHostIPLabel)


        $UsernameLabel = New-Object System.Windows.Forms.Label
        $UsernameLabel.Text = "Username:"
        $UsernameLabel.AutoSize = $True
        $UsernameLabel.Location = new-object System.Drawing.Size(10,50)
        $VMBuildoutForm.Controls.Add($UsernameLabel)


        $PasswordLabel = New-Object System.Windows.Forms.Label
        $PasswordLabel.Text = "Password:"
        $PasswordLabel.AutoSize = $True
        $PasswordLabel.Location = new-object System.Drawing.Size(100,50)
        $VMBuildoutForm.Controls.Add($PasswordLabel)


        $VMNamesLabel = New-Object System.Windows.Forms.Label
        $VMNamesLabel.Text = "Input VM Name(s):"
        $VMNamesLabel.AutoSize = $True
        $VMNamesLabel.Location = new-object System.Drawing.Size(10,150)
        $VMBuildoutForm.Controls.Add($VMNamesLabel)


        $ISORepositoryLabel = New-Object System.Windows.Forms.Label
        $ISORepositoryLabel.Text = "Select ISO Repository"
        $ISORepositoryLabel.AutoSize = $True
        $ISORepositoryLabel.Location = new-object System.Drawing.Size(10,330)
        $VMBuildoutForm.Controls.Add($ISORepositoryLabel)


        $ISOListLabel = New-Object System.Windows.Forms.Label
        $ISOListLabel.Text = "Select ISO"
        $ISOListLabel.AutoSize = $True
        $ISOListLabel.Location = new-object System.Drawing.Size(10,380)
        $VMBuildoutForm.Controls.Add($ISOListLabel)


        $RAMLabel = New-Object System.Windows.Forms.Label
        $RAMLabel.Text = "RAM Size (GB)"
        $RAMLabel.AutoSize = $True
        $RAMLabel.Location = new-object System.Drawing.Size(10,430)
        $RAMLabel.Visible = $False
        $VMBuildoutForm.Controls.Add($RAMLabel)


        $vCPULabel = New-Object System.Windows.Forms.Label
        $vCPULabel.Text = "CPU Count"
        $vCPULabel.AutoSize = $True
        $vCPULabel.Location = new-object System.Drawing.Size(160,430)
        $vCPULabel.Visible = $False
        $VMBuildoutForm.Controls.Add($vCPULabel)


        $DiskSizeLabel = New-Object System.Windows.Forms.Label
        $DiskSizeLabel.Text = "Disk Size (GB)"
        $DiskSizeLabel.AutoSize = $True
        $DiskSizeLabel.Location = new-object System.Drawing.Size(10,480)
        $DiskSizeLabel.Visible = $False
        $VMBuildoutForm.Controls.Add($DiskSizeLabel)


        $NetworkLabel = New-Object System.Windows.Forms.Label
        $NetworkLabel.Text = "Network"
        $NetworkLabel.AutoSize = $True
        $NetworkLabel.Location = new-object System.Drawing.Size(160,480)
        $NetworkLabel.Visible = $False
        $VMBuildoutForm.Controls.Add($NetworkLabel)



        #################################  DROP DOWN BOXES  #################################

        $DropDownStorage = new-object System.Windows.Forms.ComboBox
        $DropDownStorage.Location = new-object System.Drawing.Size(10,120)
        $DropDownStorage.Size = new-object System.Drawing.Size(200,20)
        $DropDownStorage.Enabled = $False
            $DropDownStorage.Add_SelectedIndexChanged({
            
                if($NewVMHostnameListBox.Items -ge 1) {
            
                GetDiskSize

                }

            $CreateNewVMButton.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($DropDownStorage)


        $DropDownTemplates = new-object System.Windows.Forms.ComboBox
        $DropDownTemplates.Location = new-object System.Drawing.Size(10,300)
        $DropDownTemplates.Size = new-object System.Drawing.Size(280,20)
        $DropDownTemplates.Enabled = $False
            $DropDownTemplates.Add_SelectedIndexChanged({
            
            $CreateNewVMButton.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($DropDownTemplates)


        $DropDownISORepository = new-object System.Windows.Forms.ComboBox
        $DropDownISORepository.Location = new-object System.Drawing.Size(10,350)
        $DropDownISORepository.Size = new-object System.Drawing.Size(280,20)
        $DropDownISORepository.Enabled = $False
            $DropDownISORepository.Add_SelectedIndexChanged({
            
            GetISOs
            $BlankTemplateCheckbox.Enabled = $True
            $CreateNewVMButton.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($DropDownISORepository)


        $DropDownISOs = new-object System.Windows.Forms.ComboBox
        $DropDownISOs.Location = new-object System.Drawing.Size(10,400)
        $DropDownISOs.Size = new-object System.Drawing.Size(280,20)
        $DropDownISOs.Enabled = $False
            $DropDownISOs.Add_SelectedIndexChanged({
            
            $CreateNewVMButton.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($DropDownISOs)


        $DropDownRAMAmmount = new-object System.Windows.Forms.ComboBox
        $DropDownRAMAmmount.Location = new-object System.Drawing.Size(10,450)
        $DropDownRAMAmmount.Size = new-object System.Drawing.Size(130,20)
        $DropDownRAMAmmount.Visible = $False
            $DropDownRAMAmmount.Add_SelectedIndexChanged({
            
            $CreateNewVMButton.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($DropDownRAMAmmount)


        $DropDownCPUCount = new-object System.Windows.Forms.ComboBox
        $DropDownCPUCount.Location = new-object System.Drawing.Size(160,450)
        $DropDownCPUCount.Size = new-object System.Drawing.Size(130,20)
        $DropDownCPUCount.Visible = $False
            $DropDownCPUCount.Add_SelectedIndexChanged({
            
            $CreateNewVMButton.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($DropDownCPUCount)


        $DropDownDiskSize = new-object System.Windows.Forms.ComboBox
        $DropDownDiskSize.Location = new-object System.Drawing.Size(10,500)
        $DropDownDiskSize.Size = new-object System.Drawing.Size(130,20)
        $DropDownDiskSize.Visible = $False
            $DropDownDiskSize.Add_SelectedIndexChanged({
            
            $CreateNewVMButton.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($DropDownDiskSize)


        $DropDownNetwork = new-object System.Windows.Forms.ComboBox
        $DropDownNetwork.Location = new-object System.Drawing.Size(160,500)
        $DropDownNetwork.Size = new-object System.Drawing.Size(130,20)
        $DropDownNetwork.Visible = $False
            $DropDownNetwork.Add_SelectedIndexChanged({
            
            $CreateNewVMButton.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($DropDownNetwork)



        #################################  CHECK BOXES  #################################

        $DefaultTemplateCheckbox = New-Object System.Windows.Forms.Checkbox 
        $DefaultTemplateCheckbox.Location = New-Object System.Drawing.Size(180,280) 
        $DefaultTemplateCheckbox.Size = New-Object System.Drawing.Size(130,20)
        $DefaultTemplateCheckbox.Text = "Default Templates"
        $DefaultTemplateCheckbox.Enabled = $False
            $DefaultTemplateCheckbox.Add_CheckStateChanged({

            GetAllTemplates
            GetServerNetworks
            GetCPUCount

                if($NewVMHostnameListBox.Items.Count -ne 0) {

                    if($DropDownStorage.SelectedItem -ne $null) {

                    GetDiskSize

                    }

                GetRAM

                }
        
                if($DefaultTemplateCheckbox.CheckState -eq "Checked") { 

                $DropDownTemplates.ResetText()

                $DropDownRAMAmmount.Visible = $True
                $DropDownCPUCount.Visible = $True
                $DropDownDiskSize.Visible = $True
                $RAMLabel.Visible = $True
                $vCPULabel.Visible = $True
                $DiskSizeLabel.Visible = $True
                $NetworkLabel.Visible = $True
                $DropDownNetwork.Visible = $True
                $VMBuildoutForm.ClientSize = new-object System.Drawing.Size(300,565)
                $CreateNewVMButton.Location = new-object System.Drawing.Size(10,535)
                $ValidateVMCreationButton.Location = new-object System.Drawing.Size(105,535)
                $ExitVMCreationButton.Location = new-object System.Drawing.Size(200,535)

                }

                else {

                $DropDownTemplates.ResetText()

                $DropDownRAMAmmount.Visible = $False
                $DropDownCPUCount.Visible = $False
                $DropDownDiskSize.Visible = $False
                $RAMLabel.Visible = $False
                $vCPULabel.Visible = $False
                $DiskSizeLabel.Visible = $False  
                $NetworkLabel.Visible = $False 
                $DropDownNetwork.Visible = $False
                $VMBuildoutForm.ClientSize = new-object System.Drawing.Size(300,465) 
                $CreateNewVMButton.Location = new-object System.Drawing.Size(10,435)
                $ValidateVMCreationButton.Location = new-object System.Drawing.Size(105,435)
                $ExitVMCreationButton.Location = new-object System.Drawing.Size(200,435)
                        
                }

            })
        $VMBuildoutForm.Controls.Add($DefaultTemplateCheckbox)


        $BlankTemplateCheckbox = New-Object System.Windows.Forms.Checkbox 
        $BlankTemplateCheckbox.Location = New-Object System.Drawing.Size(220,380) 
        $BlankTemplateCheckbox.Size = New-Object System.Drawing.Size(100,20) 
        $BlankTemplateCheckbox.Text = "Insert ISO"
        $BlankTemplateCheckbox.Enabled = $False
            $BlankTemplateCheckbox.Add_CheckStateChanged({

                if($BlankTemplateCheckbox.CheckState -eq "Checked") {

                $DropDownISOs.Enabled = $True

                }

                else {
                
                $DropDownISOs.Enabled = $False
                $DropDownISOs.ResetText()
                
                }

            })
        $VMBuildoutForm.Controls.Add($BlankTemplateCheckbox)



        #################################  BUTTONS  #################################

        $ConnectToServerButton = new-object System.Windows.Forms.Button
        $ConnectToServerButton.Location = new-object System.Drawing.Size(190,25)
        $ConnectToServerButton.Size = new-object System.Drawing.Size(80,20)
        $ConnectToServerButton.Text = "Connect"
            $ConnectToServerButton.Add_Click({

            ImportModule
            ConnectToXenHost

                if(Get-XenSession -Url "https://$($XenHostTextBox.Text)") {

                GetSR
                GetAllTemplates
                GetISOLibrary
                    
                    if($DropDownISORepository.SelectedIndex){

                    GetISOs

                    }
                
                $XenHostTextBox.Enabled = $False
                $UsernameTextBox.Enabled = $False
                $PasswordTextBox.Enabled = $False
                $DropDownStorage.Enabled = $True
                $NewVMHostnameTextBox.Enabled = $True
                $NewVMHostnameListBox.Enabled = $True
                $DropDownStorage.Enabled = $True
                $DefaultTemplateCheckbox.Enabled = $True
                $DropDownTemplates.Enabled = $True
                $DisconnectFromServerButton.Enabled = $True
                $ConnectToServerButton.Enabled = $False
                $ValidateVMCreationButton.Enabled = $True
                $RemoveVMFromListButton.Enabled = $True

                }

            })
        $VMBuildoutForm.Controls.Add($ConnectToServerButton)


        $DisconnectFromServerButton = new-object System.Windows.Forms.Button
        $DisconnectFromServerButton.Location = new-object System.Drawing.Size(190,70)
        $DisconnectFromServerButton.Size = new-object System.Drawing.Size(80,20)
        $DisconnectFromServerButton.Text = "Disconnect"
        $DisconnectFromServerButton.Enabled = $False
            $DisconnectFromServerButton.Add_Click({

            $Session | Disconnect-XenServer 

                if($DefaultTemplateCheckbox.CheckState -eq "Checked") {
                
                $DefaultTemplateCheckbox.Checked = $False
                $DefaultTemplateCheckbox.Enabled = $False
                $DropDownRAMAmmount.Items.Clear()
                $DropDownRAMAmmount.ResetText()
                $DropDownRAMAmmount.Visible = $False
                $DropDownCPUCount.Items.Clear()
                $DropDownCPUCount.ResetText()
                $DropDownCPUCount.Visible = $False
                $DropDownDiskSize.Items.Clear()
                $DropDownDiskSize.ResetText()
                $DropDownDiskSize.Visible = $False
                $DropDownNetwork.Items.Clear()
                $DropDownNetwork.ResetText()
                $DropDownNetwork.Visible = $False

                }

            $XenHostTextBox.Enabled = $True
            $UsernameTextBox.Enabled = $True
            $PasswordTextBox.Enabled = $True
            $XenHostTextBox.Clear()
            $UsernameTextBox.Clear()
            $PasswordTextBox.Clear()
            $DropDownStorage.Items.Clear()
            $DropDownStorage.ResetText()
            $DropDownStorage.Enabled = $False
            $DropDownTemplates.Items.Clear()
            $DropDownTemplates.ResetText()
            $DropDownTemplates.Enabled = $False
            $NewVMHostnameTextBox.Clear()
            $NewVMHostnameTextBox.Enabled = $False
            $BlankTemplateCheckbox.Enabled = $False
            $BlankTemplateCheckbox.Checked = $False
            $ValidateVMCreationButton.Enabled = $False
            $RemoveVMFromListButton.Enabled = $False
            $ConnectToServerButton.Enabled = $True
            $NewVMHostnameListBox.Items.Clear()
            $NewVMHostnameListBox.Enabled = $False
            $DropDownISORepository.Items.Clear()
            $DropDownISORepository.ResetText()
            $DropDownISORepository.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($DisconnectFromServerButton)


        $AddVMToListButton = new-object System.Windows.Forms.Button
        $AddVMToListButton.Location = new-object System.Drawing.Size(220,170)
        $AddVMToListButton.Size = new-object System.Drawing.Size(70,20)
        $AddVMToListButton.Text = "Add"
        $AddVMToListButton.Enabled = $False
            $AddVMToListButton.Add_Click({
            
            $AllExistingVMs = (Get-XenVM | where {$_.is_a_template -eq $False -and $_.name_label -notmatch "Control domain"}).name_label
            $AlreadyExistingVMs = $Null
            $AlreadyListedVMs = $Null
            $DuplicateNames = $False
            $ListedDuplicates = @()
            $CurentCount = $NewVMHostnameListBox.Items.Count

            $NewVMHostnames = $NewVMHostnameTextBox.text

                if($NewVMHostnameTextBox.text -match "," -and $NewVMHostnameTextBox.text -match ";") {
                
                [System.Windows.MessageBox]::Show("Error in VM Host Name TextBox: Only use commas or semi-colons, not both")
                       
                }

                else {
            
                    if($NewVMHostnameTextBox.text -match ",") {
                
                    $NewVMHostnames = $NewVMHostnameTextBox.text.split(",")
            
                    }

                    if($NewVMHostnameTextBox.text -match ";") {
            
                    $NewVMHostnames = $NewVMHostnameTextBox.text.split(";")
            
                    }

                    foreach($VMHostname in $NewVMHostnames) {
                    
                        if(($NewVMHostnames | where {$_ -match $VMHostname}).Count -ge 2) {
                        
                        $DuplicateNames = $True
                        $ListedDuplicates += "$VMHostname`n"

                        }
                                        
                    }

                    if($DuplicateNames -eq $False) {

                        foreach($VMHostname in $NewVMHostnames) {

                            if($AllExistingVMs -notcontains $VMHostname -and $NewVMHostnameListBox.Items -notcontains $VMHostname) {

                            $NewVMHostnameListBox.Items.Add($VMHostname)

                            }

                            elseif($NewVMHostnameListBox.Items -contains $VMHostname) {
                            
                            $AlreadyListedVMs += "$VMHostname`n"
                            
                            } 

                            elseif($AllExistingVMs -contains $VMHostname) {
                    
                            $AlreadyExistingVMs += "$VMHostname`n"
                    
                            }

                        }

                        if($AlreadyExistingVMs -ne $Null) {
                
                        [System.Windows.MessageBox]::Show("Error in VM Host Name TextBox: The following server names already exist in XenCenter and were not added as a result:`n`n$AlreadyExistingVMs")
                                
                        }

                        elseif($AlreadyListedVMs -ne $Null) {
                        
                        [System.Windows.MessageBox]::Show("Error in VM Host Name TextBox: The following server names are already listed and were not added as a result:`n`n$AlreadyListedVMs")
                         
                        }

                        if($CurentCount -ne $NewVMHostnameListBox.Items.Count) {
                        
                            if($DropDownStorage.SelectedItem -ne $Null) {

                            GetDiskSize

                            }

                        GetRAM

                        $NewVMHostnameTextBox.Clear()
                        $CreateNewVMButton.Enabled = $False                        
                        
                        }

                    }

                    else {
                    
                    [System.Windows.MessageBox]::Show("Error in VM Host Name TextBox: Duplicate names discovered, remove them to continue:`n`n$($ListedDuplicates | Get-Unique)")
                           
                    }

                }

            })
        $VMBuildoutForm.Controls.Add($AddVMToListButton)


        $RemoveVMFromListButton = new-object System.Windows.Forms.Button
        $RemoveVMFromListButton.Location = new-object System.Drawing.Size(220,200)
        $RemoveVMFromListButton.Size = new-object System.Drawing.Size(70,20)
        $RemoveVMFromListButton.Text = "Remove"
        $RemoveVMFromListButton.Enabled = $False
            $RemoveVMFromListButton.Add_Click({

            $NewVMHostnameListBox.Items.Remove($NewVMHostnameListBox.SelectedItem)

                if($NewVMHostnameListBox.Items.Count -lt 1 -and $DefaultTemplateCheckbox.CheckState -eq 'Checked') {
                
                $DropDownDiskSize.Items.Clear()
                $DropDownDiskSize.ResetText()
                $DropDownRAMAmmount.Items.Clear()
                $DropDownRAMAmmount.ResetText()
                $DropDownCPUCount.ResetText()
                $DropDownNetwork.ResetText()

                }

                if($DropDownStorage.SelectedItem -ne $null -and $DropDownDiskSize.SelectedItem -ne $null) {

                GetDiskSize

                }

                if($DropDownRAMAmmount.SelectedItem -ne $null) {

                GetRAM

                }

            $CreateNewVMButton.Enabled = $False

            })
        $VMBuildoutForm.Controls.Add($RemoveVMFromListButton)


        $CreateNewVMButton = new-object System.Windows.Forms.Button
        $CreateNewVMButton.Location = new-object System.Drawing.Size(10,435)
        $CreateNewVMButton.Size = new-object System.Drawing.Size(85,20)
        $CreateNewVMButton.Text = "Create"
        $CreateNewVMButton.Enabled = $False
            $CreateNewVMButton.Add_Click({

                if($DefaultTemplateCheckbox.CheckState -eq 'Unchecked') {
            
                $VMBuildoutForm.Height = 580
                $CreateNewVMButton.Location = new-object System.Drawing.Size(10,510)
                $ValidateVMCreationButton.Location = new-object System.Drawing.Size(105,510)
                $ExitVMCreationButton.Location = new-object System.Drawing.Size(205,510)
                $VMCreationProgressTextBox.Visible = $True

                }
            
                elseif($DefaultTemplateCheckbox.CheckState -eq 'Checked') {

                $VMBuildoutForm.Height = 680                
                $CreateNewVMButton.Location = new-object System.Drawing.Size(10,610)
                $ValidateVMCreationButton.Location = new-object System.Drawing.Size(105,610)
                $ExitVMCreationButton.Location = new-object System.Drawing.Size(205,610)
                $VMCreationProgressTextBox.Location = new-object System.Drawing.Size(10,530)
                $VMCreationProgressTextBox.Visible = $True                
                           
                }

            BuildVMs
            CreateAdditionalVMsForm

            })
        $VMBuildoutForm.Controls.Add($CreateNewVMButton)


        $ValidateVMCreationButton = new-object System.Windows.Forms.Button
        $ValidateVMCreationButton.Location = new-object System.Drawing.Size(105,435)
        $ValidateVMCreationButton.Size = new-object System.Drawing.Size(90,20)
        $ValidateVMCreationButton.Text = "Validate"
        $ValidateVMCreationButton.Enabled = $False
            $ValidateVMCreationButton.Add_Click({

            CheckVMFormProperties

            })
        $VMBuildoutForm.Controls.Add($ValidateVMCreationButton)


        $ExitVMCreationButton = new-object System.Windows.Forms.Button
        $ExitVMCreationButton.Location = new-object System.Drawing.Size(205,435)
        $ExitVMCreationButton.Size = new-object System.Drawing.Size(85,20)
        $ExitVMCreationButton.Text = "Exit"
            $ExitVMCreationButton.Add_Click({

            DisconnectXenHost
            $VMBuildoutForm.Close()

            })
        $VMBuildoutForm.Controls.Add($ExitVMCreationButton)


        #################################  TEXT BOXES  #################################

        $XenHostTextBox = new-object System.Windows.Forms.TextBox
        $XenHostTextBox.Location = new-object System.Drawing.Size(10,25)
        $XenHostTextBox.Size = new-object System.Drawing.Size(170,20)
        $VMBuildoutForm.Controls.Add($XenHostTextBox)


        $UsernameTextBox = new-object System.Windows.Forms.TextBox
        $UsernameTextBox.Location = new-object System.Drawing.Size(10,70)
        $UsernameTextBox.Size = new-object System.Drawing.Size(80,20)
        $VMBuildoutForm.Controls.Add($UsernameTextBox)


        $PasswordTextBox = new-object System.Windows.Forms.MaskedTextBox
        $PasswordTextBox.Location = new-object System.Drawing.Size(100,70)
        $PasswordTextBox.Size = new-object System.Drawing.Size(80,20)
        $PasswordTextBox.PasswordChar = '*'
        $VMBuildoutForm.Controls.Add($PasswordTextBox)


        $NewVMHostnameTextBox = new-object System.Windows.Forms.TextBox
        $NewVMHostnameTextBox.Location = new-object System.Drawing.Size(10,170)
        $NewVMHostnameTextBox.Size = new-object System.Drawing.Size(200,20)
        $NewVMHostnameTextBox.Enabled = $False
            $NewVMHostnameTextBox.Add_TextChanged({
        
                if($NewVMHostnameTextBox.Text.Length -gt 0) {
                
                $AddVMToListButton.Enabled = $True
                
                }

                else {
                
                $AddVMToListButton.Enabled = $False
                
                }
        
            })
        $VMBuildoutForm.Controls.Add($NewVMHostnameTextBox)
        

        $VMCreationProgressTextBox = new-object System.Windows.Forms.TextBox
        $VMCreationProgressTextBox.Location = new-object System.Drawing.Size(10,430)
        $VMCreationProgressTextBox.Size = new-object System.Drawing.Size(280,70)
        $VMCreationProgressTextBox.Visible = $False
        $VMCreationProgressTextBox.Multiline = $True
        $VMBuildoutForm.Controls.Add($VMCreationProgressTextBox)

        

        #################################  LIST BOXES  #################################

        $NewVMHostnameListBox = new-object System.Windows.Forms.ListBox
        $NewVMHostnameListBox.Location = new-object System.Drawing.Size(10,200)
        $NewVMHostnameListBox.Size = new-object System.Drawing.Size(200,80)
        $NewVMHostnameListBox.Enabled = $False
        $VMBuildoutForm.Controls.Add($NewVMHostnameListBox)


$VMBuildoutForm.ShowDialog()

#################################  END FORM  #################################

}


Function XenServerModuleForm {

#################################  XENSERVER MODULE FORM  #################################

$XenServerModuleForm = New-Object system.Windows.Forms.Form
$XenServerModuleForm.Text = "AXL"
$XenServerModuleForm.ClientSize = new-object System.Drawing.Size(285,110)
$XenServerModuleForm.FormBorderStyle = 'Fixed3D'
$XenServerModuleForm.MaximizeBox = $False
$XenServerModuleForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $DomainDecsiionLabel = New-Object System.Windows.Forms.Label
        $DomainDecsiionLabel.Text = "Please Input the Location of the XenServer Module:`n(XenServerPowerShell.dll)"
        $DomainDecsiionLabel.AutoSize = $True
        $DomainDecsiionLabel.Location = new-object System.Drawing.Size(15,10)
        $XenServerModuleForm.Controls.Add($DomainDecsiionLabel)



        #################################  BUTTONS  #################################

        $ModuleContinueButton = new-object System.Windows.Forms.Button
        $ModuleContinueButton.Location = new-object System.Drawing.Size(15,80)
        $ModuleContinueButton.Size = new-object System.Drawing.Size(110,20)
        $ModuleContinueButton.Text = "Continue"
        $ModuleContinueButton.Enabled = $False
            $ModuleContinueButton.Add_Click({
          
            $XenServerModuleForm.Close()

            })
        $XenServerModuleForm.Controls.Add($ModuleContinueButton) 


        $ModuleValidateButton = new-object System.Windows.Forms.Button
        $ModuleValidateButton.Location = new-object System.Drawing.Size(160,80)
        $ModuleValidateButton.Size = new-object System.Drawing.Size(110,20)
        $ModuleValidateButton.Text = "Validate"
            $ModuleValidateButton.Add_Click({

            ValidateModulePath

            })
        $XenServerModuleForm.Controls.Add($ModuleValidateButton)

        
        $ModuleBrowseButton = new-object System.Windows.Forms.Button
        $ModuleBrowseButton.Location = new-object System.Drawing.Size(210,50)
        $ModuleBrowseButton.Size = new-object System.Drawing.Size(60,20)
        $ModuleBrowseButton.Text = "Browse..."
            $ModuleBrowseButton.Add_Click({

            $ModuleLocationTextBox.Text = BrowseFiles

            })
        $XenServerModuleForm.Controls.Add($ModuleBrowseButton)



        #################################  TEXT BOXES  #################################

        $ModuleLocationTextBox = new-object System.Windows.Forms.TextBox
        $ModuleLocationTextBox.Location = new-object System.Drawing.Size(15,50)
        $ModuleLocationTextBox.Size = new-object System.Drawing.Size(190,20)
            $ModuleLocationTextBox.Add_TextChanged({
            
            $ModuleContinueButton.Enabled = $False

            })
        $XenServerModuleForm.Controls.Add($ModuleLocationTextBox)


$XenServerModuleForm.ShowDialog()

#################################  END FORM  #################################

}


Function CreateAdditionalVMsForm {

#################################  CREATE ADDITIONAL VMS FORM  #################################

$CreateAdditionalVMsForm = New-Object system.Windows.Forms.Form
$CreateAdditionalVMsForm.Text = "AXL"
$CreateAdditionalVMsForm.ClientSize = new-object System.Drawing.Size(220,60)
$CreateAdditionalVMsForm.FormBorderStyle = 'Fixed3D'
$CreateAdditionalVMsForm.MaximizeBox = $False
$CreateAdditionalVMsForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $MoreVMsLabel = New-Object System.Windows.Forms.Label
        $MoreVMsLabel.Text = "Would you like to create more VMs?"
        $MoreVMsLabel.AutoSize = $True
        $MoreVMsLabel.Location = new-object System.Drawing.Size(20,5)
        $CreateAdditionalVMsForm.Controls.Add($MoreVMsLabel)



        #################################  BUTTONS  #################################


        $MoreVMsYesButton = new-object System.Windows.Forms.Button
        $MoreVMsYesButton.Location = new-object System.Drawing.Size(10,30)
        $MoreVMsYesButton.Size = new-object System.Drawing.Size(90,20)
        $MoreVMsYesButton.Text = "Yes"
            $MoreVMsYesButton.Add_Click({

            $VMCreationProgressTextBox.Clear()
            $VMCreationProgressTextBox.Visible = $False
            $DropDownStorage.ResetText()
            $DropDownTemplates.ResetText()
            $NewVMHostnameTextBox.Clear()
            $NewVMHostnameListBox.Items.Clear()
            $CreateNewVMButton.Enabled = $False
            $CreateNewVMButton.Location = new-object System.Drawing.Size(10,435)
            $ValidateVMCreationButton.Location = new-object System.Drawing.Size(105,435)
            $ExitVMCreationButton.Location = new-object System.Drawing.Size(205,435)

                if($DefaultTemplateCheckbox.CheckState -eq 'Checked') {

                $DropDownDiskSize.ResetText()
                $DropDownRAMAmmount.ResetText()
                $DropDownCPUCount.ResetText()
                $DropDownNetwork.ResetText()
                $DefaultTemplateCheckbox.Checked = $False

                }

                if($BlankTemplateCheckbox.CheckState -eq 'Checked') {
                
                $DropDownISOs.ResetText()
                $BlankTemplateCheckbox.Checked = $False
                
                }

                if(($DropDownISORepository.Items).count -gt 1) {
                
                $DropDownISORepository.ResetText()

                }
            
            $VMBuildoutForm.ClientSize = new-object System.Drawing.Size(300,465)
            $Global:StopWatch.Reset()

            $CreateAdditionalVMsForm.Close()

            })
        $CreateAdditionalVMsForm.Controls.Add($MoreVMsYesButton) 


        $MoreVMsNoButton = new-object System.Windows.Forms.Button
        $MoreVMsNoButton.Location = new-object System.Drawing.Size(120,30)
        $MoreVMsNoButton.Size = new-object System.Drawing.Size(90,20)
        $MoreVMsNoButton.Text = "No"
            $MoreVMsNoButton.Add_Click({

            $CreateAdditionalVMsForm.Hide()
            $VMBuildoutForm.Hide()
            DomainDecisionForm

            })
        $CreateAdditionalVMsForm.Controls.Add($MoreVMsNoButton) 


$CreateAdditionalVMsForm.ShowDialog()

#################################  END FORM  #################################

}


Function DomainDecisionForm {

#################################  DOMAIN DECISION FORM  #################################

$DomainDecisionForm = New-Object system.Windows.Forms.Form
$DomainDecisionForm.Text = "AXL"
$DomainDecisionForm.ClientSize = new-object System.Drawing.Size(240,80)
$DomainDecisionForm.FormBorderStyle = 'Fixed3D'
$DomainDecisionForm.MaximizeBox = $False
$DomainDecisionForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $DomainDecisionLabel = New-Object System.Windows.Forms.Label
        $DomainDecisionLabel.Text = "Would you Like to Configure Active Directory and Other Services?"
        $DomainDecisionLabel.Size = new-object System.Drawing.Size(220,40)
        $DomainDecisionLabel.Location = new-object System.Drawing.Size(10,5)
        $DomainDecisionLabel.TextAlign = "MiddleCenter"
        $DomainDecisionForm.Controls.Add($DomainDecisionLabel)



        #################################  BUTTONS  #################################


        $DomainDecisionYesButton = new-object System.Windows.Forms.Button
        $DomainDecisionYesButton.Location = new-object System.Drawing.Size(10,50)
        $DomainDecisionYesButton.Size = new-object System.Drawing.Size(100,20)
        $DomainDecisionYesButton.Text = "Yes"
            $DomainDecisionYesButton.Add_Click({
            
            StartCheckJobs
            $DomainDecisionForm.Hide()
            ComponentInstallationForm

            })
        $DomainDecisionForm.Controls.Add($DomainDecisionYesButton) 


        $DomainDecisionNoButton = new-object System.Windows.Forms.Button
        $DomainDecisionNoButton.Location = new-object System.Drawing.Size(130,50)
        $DomainDecisionNoButton.Size = new-object System.Drawing.Size(100,20)
        $DomainDecisionNoButton.Text = "No"
            $DomainDecisionNoButton.Add_Click({

            DisconnectXenHost
            $DomainDecisionForm.Close()
            $VMBuildoutForm.Close()

            })
        $DomainDecisionForm.Controls.Add($DomainDecisionNoButton) 


$DomainDecisionForm.ShowDialog()

#################################  END FORM  #################################

}


Function StatusForm {

#################################  STATUS FORM  #################################

$StatusForm = New-Object system.Windows.Forms.Form
$StatusForm.Text = "AXL"
$StatusForm.ClientSize = new-object System.Drawing.Size(390,355)
$StatusForm.FormBorderStyle = 'Fixed3D'
$StatusForm.MaximizeBox = $False
$StatusForm.StartPosition = 'CenterScreen'
    

        #################################  LABELS  #################################

        $ServerStatusLabel = New-Object System.Windows.Forms.Label
        $ServerStatusLabel.Text = "Please Press 'Create' to Continue"
        $ServerStatusLabel.Size = new-object System.Drawing.Size(370,20)
        $ServerStatusLabel.TextAlign = "MiddleCenter"
        $ServerStatusLabel.Location = new-object System.Drawing.Size(10,295)
        $StatusForm.Controls.Add($ServerStatusLabel)



        #################################  BUTTONS  #################################

        $CompleteStatusButton = new-object System.Windows.Forms.Button
        $CompleteStatusButton.Location = new-object System.Drawing.Size(95,320)
        $CompleteStatusButton.Size = new-object System.Drawing.Size(200,20)
        $CompleteStatusButton.Text = "Complete"
        $CompleteStatusButton.visible = $False
            $CompleteStatusButton.Add_Click({

            $StatusForm.Close()

            })
        $StatusForm.Controls.Add($CompleteStatusButton) 


        $CheckStatusButton = new-object System.Windows.Forms.Button
        $CheckStatusButton.Location = new-object System.Drawing.Size(95,320)
        $CheckStatusButton.Size = new-object System.Drawing.Size(200,20)
        $CheckStatusButton.Text = "Create"
            $CheckStatusButton.Add_Click({

            $CheckStatusButton.visible = $False
            $ServerStatusLabel.Size = new-object System.Drawing.Size(370,60)
            CheckServerIPState
            WaitScript 15
            CompleteBuildout
            $ServerStatusLabel.Size = new-object System.Drawing.Size(370,20)
            $CompleteStatusButton.visible = $True

            })
        $StatusForm.Controls.Add($CheckStatusButton) 


        #################################  TEXT BOXES  #################################

        $VMStatusTextBox = new-object System.Windows.Forms.TextBox
        $VMStatusTextBox.Location = new-object System.Drawing.Size(10,10)
        $VMStatusTextBox.Size = new-object System.Drawing.Size(370,280)
        $VMStatusTextBox.Visible = $False
        $VMStatusTextBox.Multiline = $True
        $VMStatusTextBox.ScrollBars = 3
        $StatusForm.Controls.Add($VMStatusTextBox)



        #################################  LIST VIEW  #################################

        $CheckStateListView = new-object System.Windows.Forms.ListView
        $CheckStateListView.Location = new-object System.Drawing.Size(10,10)
        $CheckStateListView.Size = new-object System.Drawing.Size(370,280)
        $CheckStateListView.Columns.Add("Server",150) | Out-Null
        $CheckStateListView.Columns.Add("Status",90) | Out-Null
        $CheckStateListView.Columns.Add("IP Address",125) | Out-Null
        $CheckStateListView.View = "Details"
        $StatusForm.Controls.Add($CheckStateListView)

        foreach($CheckStateServer in ($Global:AllCreatedServers | sort)){

        $ListViewItem = New-Object System.Windows.Forms.ListViewItem($CheckStateServer)
        $ListViewItem.Subitems.Add("In Progress") | Out-Null
        $ListViewItem.Subitems.Add("Unknown") | Out-Null
        $CheckStateListView.Items.Add($ListViewItem) | Out-Null

        }


$StatusForm.ShowDialog()

#################################  END FORM  #################################

}


Function DomainBuildoutForm {

#################################  DOMAIN BUILDOUT FORM  #################################

$DomainBuildoutForm = New-Object system.Windows.Forms.Form
$DomainBuildoutForm.Text = "AXL - Domain Buildout"
$DomainBuildoutForm.ClientSize = new-object System.Drawing.Size(390,470)
$DomainBuildoutForm.FormBorderStyle = 'Fixed3D'
$DomainBuildoutForm.MaximizeBox = $False
$DomainBuildoutForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $LocalUsernameLabel = New-Object System.Windows.Forms.Label
        $LocalUsernameLabel.Text = "Local Admin Username:"
        $LocalUsernameLabel.AutoSize = $True
        $LocalUsernameLabel.Location = new-object System.Drawing.Size(10,15)
        $DomainBuildoutForm.Controls.Add($LocalUsernameLabel)        
        

        $IPSchemaLabel = New-Object System.Windows.Forms.Label
        $IPSchemaLabel.Text = "========================  IP Schema  ======================"
        $IPSchemaLabel.AutoSize = $True
        $IPSchemaLabel.Location = new-object System.Drawing.Size(10,115)
        $DomainBuildoutForm.Controls.Add($IPSchemaLabel)

        
        $LocalPasswordLabel = New-Object System.Windows.Forms.Label
        $LocalPasswordLabel.Text = "Local Admin Password:"
        $LocalPasswordLabel.AutoSize = $True
        $LocalPasswordLabel.Location = new-object System.Drawing.Size(220,15)
        $DomainBuildoutForm.Controls.Add($LocalPasswordLabel)        

        
        $DomainNameLabel = New-Object System.Windows.Forms.Label
        $DomainNameLabel.Text = "Input a Domain Name:"
        $DomainNameLabel.AutoSize = $True
        $DomainNameLabel.Location = new-object System.Drawing.Size(10,65)
        $DomainBuildoutForm.Controls.Add($DomainNameLabel)


        $SafeModeLabel = New-Object System.Windows.Forms.Label
        $SafeModeLabel.Text = "Safe Mode Password:"
        $SafeModeLabel.AutoSize = $True
        $SafeModeLabel.Location = new-object System.Drawing.Size(220,65)
        $DomainBuildoutForm.Controls.Add($SafeModeLabel) 


        $DefaultGatewayLabel = New-Object System.Windows.Forms.Label
        $DefaultGatewayLabel.Text = "Default Gateway:"
        $DefaultGatewayLabel.AutoSize = $True
        $DefaultGatewayLabel.Location = new-object System.Drawing.Size(160,140)
        $DomainBuildoutForm.Controls.Add($DefaultGatewayLabel)


        $SubnetMaskLabel = New-Object System.Windows.Forms.Label
        $SubnetMaskLabel.Text = "Subnet Mask:"
        $SubnetMaskLabel.AutoSize = $True
        $SubnetMaskLabel.Location = new-object System.Drawing.Size(275,140)
        $DomainBuildoutForm.Controls.Add($SubnetMaskLabel)


        $IPAddressLabel = New-Object System.Windows.Forms.Label
        $IPAddressLabel.Text = "IP Address:"
        $IPAddressLabel.AutoSize = $True
        $IPAddressLabel.Location = new-object System.Drawing.Size(160,190)
        $DomainBuildoutForm.Controls.Add($IPAddressLabel)


        $PrimaryDNSLabel = New-Object System.Windows.Forms.Label
        $PrimaryDNSLabel.Text = "Primary DNS:"
        $PrimaryDNSLabel.AutoSize = $True
        $PrimaryDNSLabel.Location = new-object System.Drawing.Size(160,240)
        $DomainBuildoutForm.Controls.Add($PrimaryDNSLabel)


        $SecondaryDNSLabel = New-Object System.Windows.Forms.Label
        $SecondaryDNSLabel.Text = "Secondary DNS:"
        $SecondaryDNSLabel.AutoSize = $True
        $SecondaryDNSLabel.Location = new-object System.Drawing.Size(275,240)
        $DomainBuildoutForm.Controls.Add($SecondaryDNSLabel)


        $DomainControllersSectionLabel = New-Object System.Windows.Forms.Label
        $DomainControllersSectionLabel.Text = "================  Domain Controller Selection  ================="
        $DomainControllersSectionLabel.AutoSize = $True
        $DomainControllersSectionLabel.Location = new-object System.Drawing.Size(10,285)
        $DomainBuildoutForm.Controls.Add($DomainControllersSectionLabel)


        $AllMembersLabel = New-Object System.Windows.Forms.Label
        $AllMembersLabel.Text = "All Members"
        $AllMembersLabel.AutoSize = $True
        $AllMembersLabel.Location = new-object System.Drawing.Size(45,300)
        $DomainBuildoutForm.Controls.Add($AllMembersLabel)


        $DomainControllersLabel = New-Object System.Windows.Forms.Label
        $DomainControllersLabel.Text = "Domain Controllers"
        $DomainControllersLabel.AutoSize = $True
        $DomainControllersLabel.Location = new-object System.Drawing.Size(260,300)
        $DomainBuildoutForm.Controls.Add($DomainControllersLabel)



        #################################  TEXT BOXES  #################################

        $LocalUsernameTextBox = new-object System.Windows.Forms.TextBox
        $LocalUsernameTextBox.Location = new-object System.Drawing.Size(10,35)
        $LocalUsernameTextBox.Size = new-object System.Drawing.Size(160,20)
            $LocalUsernameTextBox.Add_TextChanged({

            $DomainBuildoutButton.Enabled = $False
                
            })
        $DomainBuildoutForm.Controls.Add($LocalUsernameTextBox)


        $LocalPasswordTextBox = new-object System.Windows.Forms.TextBox
        $LocalPasswordTextBox.Location = new-object System.Drawing.Size(220,35)
        $LocalPasswordTextBox.Size = new-object System.Drawing.Size(160,20)
        $LocalPasswordTextBox.PasswordChar = '*'
            $LocalPasswordTextBox.Add_TextChanged({

            $DomainBuildoutButton.Enabled = $False
                
            })
        $DomainBuildoutForm.Controls.Add($LocalPasswordTextBox)        
        
        
        $DomainNameTextBox = new-object System.Windows.Forms.TextBox
        $DomainNameTextBox.Location = new-object System.Drawing.Size(10,85)
        $DomainNameTextBox.Size = new-object System.Drawing.Size(160,20)
            $DomainNameTextBox.Add_TextChanged({

            $DomainBuildoutButton.Enabled = $False
                
            })
        $DomainBuildoutForm.Controls.Add($DomainNameTextBox)


        $SafeModePasswordTextBox = new-object System.Windows.Forms.TextBox
        $SafeModePasswordTextBox.Location = new-object System.Drawing.Size(220,85)
        $SafeModePasswordTextBox.Size = new-object System.Drawing.Size(160,20)
        $SafeModePasswordTextBox.PasswordChar = '*'
            $SafeModePasswordTextBox.Add_TextChanged({

            $DomainBuildoutButton.Enabled = $False
                
            })
        $DomainBuildoutForm.Controls.Add($SafeModePasswordTextBox)  


        $DefaultGatewayTextBox = new-object System.Windows.Forms.TextBox
        $DefaultGatewayTextBox.Location = new-object System.Drawing.Size(160,160)
        $DefaultGatewayTextBox.Size = new-object System.Drawing.Size(105,20)
        $DefaultGatewayTextBox.Enabled = $False
            $DefaultGatewayTextBox.Add_TextChanged({
            
            DefinePreviouslySelectedIndex

                if($DefaultGatewayTextBox.Text) {
    
                $Global:DefaultGateways[$IPSchemaListBox.SelectedIndex] = $DefaultGatewayTextBox.Text
    
                }
            
            $DomainBuildoutButton.Enabled = $False
                
            })
        $DomainBuildoutForm.Controls.Add($DefaultGatewayTextBox)
        

        $SubnetMaskTextBox = new-object System.Windows.Forms.TextBox
        $SubnetMaskTextBox.Location = new-object System.Drawing.Size(275,160)
        $SubnetMaskTextBox.Size = new-object System.Drawing.Size(105,20)
        $SubnetMaskTextBox.Enabled = $False
            $SubnetMaskTextBox.Add_TextChanged({
            
            DefinePreviouslySelectedIndex

                if($SubnetMaskTextBox.Text -ne $null) {

                $Global:SubnetMasks[$IPSchemaListBox.SelectedIndex] = $SubnetMaskTextBox.Text

                }
            
            $DomainBuildoutButton.Enabled = $False

            })
        $DomainBuildoutForm.Controls.Add($SubnetMaskTextBox)


        $IPAddressTextBox = new-object System.Windows.Forms.TextBox
        $IPAddressTextBox.Location = new-object System.Drawing.Size(160,210)
        $IPAddressTextBox.Size = new-object System.Drawing.Size(220,20)
        $IPAddressTextBox.Enabled = $False
            $IPAddressTextBox.Add_TextChanged({
            
            DefinePreviouslySelectedIndex

                if($IPAddressTextBox.Text -ne $null) {
    
                $Global:IPAddresses[$IPSchemaListBox.SelectedIndex] = $IPAddressTextBox.Text
    
                }
            
            $DomainBuildoutButton.Enabled = $False

            })
        $DomainBuildoutForm.Controls.Add($IPAddressTextBox)


        $PrimaryDNSTextBox = new-object System.Windows.Forms.TextBox
        $PrimaryDNSTextBox.Location = new-object System.Drawing.Size(160,260)
        $PrimaryDNSTextBox.Size = new-object System.Drawing.Size(105,20)
        $PrimaryDNSTextBox.Enabled = $False
            $PrimaryDNSTextBox.Add_TextChanged({
            
            DefinePreviouslySelectedIndex

                if($PrimaryDNSTextBox.Text -ne $null) {
    
                $Global:PrimaryDNSServers[$IPSchemaListBox.SelectedIndex] = $PrimaryDNSTextBox.Text
    
                }

                if($PrimaryDNSTextBox.Text -match [regex]"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"){
                
                $SecondaryDNSTextBox.Enabled = $True
                
                }

                else{
                
                $SecondaryDNSTextBox.Enabled = $False
                $SecondaryDNSTextBox.Clear()

                }

            $DomainBuildoutButton.Enabled = $False

            })
        $DomainBuildoutForm.Controls.Add($PrimaryDNSTextBox)


        $SecondaryDNSTextBox = new-object System.Windows.Forms.TextBox
        $SecondaryDNSTextBox.Location = new-object System.Drawing.Size(275,260)
        $SecondaryDNSTextBox.Size = new-object System.Drawing.Size(105,20)
        $SecondaryDNSTextBox.Enabled = $False
            $SecondaryDNSTextBox.Add_TextChanged({
            
            DefinePreviouslySelectedIndex

                if($SecondaryDNSTextBox.Text -ne $null) {
    
                $Global:SecondaryDNSServers[$IPSchemaListBox.SelectedIndex] = $SecondaryDNSTextBox.Text
    
                }
            
            $DomainBuildoutButton.Enabled = $False

            })
        $DomainBuildoutForm.Controls.Add($SecondaryDNSTextBox)

        

        #################################  BUTTONS  #################################

        $DomainBuildoutButton = new-object System.Windows.Forms.Button
        $DomainBuildoutButton.Location = new-object System.Drawing.Size(10,440)
        $DomainBuildoutButton.Size = new-object System.Drawing.Size(110,20)
        $DomainBuildoutButton.Text = "Next $($Global:FinalForms.IndexOf("Domain")+2) / $($Global:FinalForms.Count)"
        $DomainBuildoutButton.Enabled = $False
            $DomainBuildoutButton.Add_Click({
          
            $DomainBuildoutForm.Hide()
            UserGroupOUForm

            })
        $DomainBuildoutForm.Controls.Add($DomainBuildoutButton) 


        $DomainBuildoutValidateButton = new-object System.Windows.Forms.Button
        $DomainBuildoutValidateButton.Location = new-object System.Drawing.Size(140,440)
        $DomainBuildoutValidateButton.Size = new-object System.Drawing.Size(115,20)
        $DomainBuildoutValidateButton.Text = "Validate"
            $DomainBuildoutValidateButton.Add_Click({

            ValidateDomainCreation

            })
        $DomainBuildoutForm.Controls.Add($DomainBuildoutValidateButton) 


        $DomainBuildoutExitButton = new-object System.Windows.Forms.Button
        $DomainBuildoutExitButton.Location = new-object System.Drawing.Size(270,440)
        $DomainBuildoutExitButton.Size = new-object System.Drawing.Size(110,20)
        $DomainBuildoutExitButton.Text = "Exit"
            $DomainBuildoutExitButton.Add_Click({

            DisconnectXenHost
            $DomainBuildoutForm.Close()

            })
        $DomainBuildoutForm.Controls.Add($DomainBuildoutExitButton) 


        $AddServerButton = new-object System.Windows.Forms.Button
        $AddServerButton.Location = new-object System.Drawing.Size(175,330)
        $AddServerButton.Size = new-object System.Drawing.Size(40,20)
        $AddServerButton.Text = ">>>"
            $AddServerButton.Add_Click({

                if($AllServersListBox.SelectedItem){
            
                $DomainControllersListBox.Items.Add($AllServersListBox.SelectedItem)
                $AllServersListBox.Items.Remove($AllServersListBox.SelectedItem)
                $AllServersListBox.ClearSelected()
                $DomainControllersListBox.ClearSelected()
                
                $PrimaryDCListedServers = @()
                $Counter = 0

                    while($Counter -lt $DomainControllersListBox.Items.Count) {
                
                    $PrimaryDCListedServers += $DomainControllersListBox.Items[$Counter]

                    $Counter++

                    }

                    $DomainControllersListBox.Items.Clear()

                    foreach($PrimaryDCListedServer in ($PrimaryDCListedServers | Sort)) {
                    
                    $DomainControllersListBox.Items.Add($PrimaryDCListedServer)

                    }

                }

            EnableMakeDCPrimary
            $DomainBuildoutButton.Enabled = $False

            })
        $DomainBuildoutForm.Controls.Add($AddServerButton) 


        $RemoveServerButton = new-object System.Windows.Forms.Button
        $RemoveServerButton.Location = new-object System.Drawing.Size(175,360)
        $RemoveServerButton.Size = new-object System.Drawing.Size(40,20)
        $RemoveServerButton.Text = "<<<"
            $RemoveServerButton.Add_Click({
            
                if($DomainControllersListBox.SelectedItem){
            
                    if($DomainControllersListBox.SelectedItem -match [regex]"\*") {

                    $AllServersListBox.Items.Add(($DomainControllersListBox.SelectedItem).Replace("*",""))
                    $DomainControllersListBox.Items.Remove($DomainControllersListBox.SelectedItem)

                    }

                    else {
                    
                    $AllServersListBox.Items.Add($DomainControllersListBox.SelectedItem)
                    $DomainControllersListBox.Items.Remove($DomainControllersListBox.SelectedItem)                 
                    
                    }

                $AllServersListBox.ClearSelected()
                $DomainControllersListBox.ClearSelected()

                $ListedServers = @()
                $Counter = 0
                
                    while($Counter -lt $AllServersListBox.Items.Count) {
                
                    $ListedServers += $AllServersListBox.Items[$Counter]

                    $Counter++
                    
                    }

                    $AllServersListBox.Items.Clear()
                    
                    foreach($ListedServer in ($ListedServers | Sort)) {
                    
                    $AllServersListBox.Items.Add($ListedServer)

                    }

                }

            EnableMakeDCPrimary
            $DomainBuildoutButton.Enabled = $False

            })
        $DomainBuildoutForm.Controls.Add($RemoveServerButton) 


        $MakePrimaryButton = new-object System.Windows.Forms.Button
        $MakePrimaryButton.Location = new-object System.Drawing.Size(165,390)
        $MakePrimaryButton.Size = new-object System.Drawing.Size(60,30)
        $MakePrimaryButton.Text = "  Make`nPrimary"
        $MakePrimaryButton.Enabled = $False
            $MakePrimaryButton.Add_Click({
            
                if($DomainControllersListBox.SelectedItem){

                $OldDCName = $DomainControllersListBox.SelectedItem
                $Counter = 0

                    if($DomainControllersListBox.Text -match [regex]"\*") {

                    $DomainControllersListBox.Items.Remove($DomainControllersListBox.Text)
                    $DomainControllersListBox.Items.Add($OldDCName.Replace("*",""))
                    $DomainControllersListBox.ClearSelected()

                    }

                    else {

                       while($Counter -lt $DomainControllersListBox.Items.Count) {
                        
                            if($DomainControllersListBox.Items[$Counter] -match [regex]"\*"){

                            $NewPrimaryDC = $DomainControllersListBox.Items[$Counter].Replace("*","")
                            
                            $DomainControllersListBox.Items.Remove($DomainControllersListBox.Items[$Counter])
                            $DomainControllersListBox.Items.Add($NewPrimaryDC)
                               
                            }

                        $Counter++
                        
                        }
                    
                    $DomainControllersListBox.Items.Remove($DomainControllersListBox.SelectedItem)
                    $DomainControllersListBox.Items.Add("$OldDCName*")
                    $DomainControllersListBox.ClearSelected()

                    }

                $PrimaryDCListedServers = @()
                $Counter = 0

                    while($Counter -lt $DomainControllersListBox.Items.Count) {
                
                    $PrimaryDCListedServers += $DomainControllersListBox.Items[$Counter]

                    $Counter++

                    }

                    $DomainControllersListBox.Items.Clear()

                    foreach($PrimaryDCListedServer in ($PrimaryDCListedServers | Sort)) {
                    
                    $DomainControllersListBox.Items.Add($PrimaryDCListedServer)

                    }

                }

            })
        $DomainBuildoutForm.Controls.Add($MakePrimaryButton) 



        #################################  LIST BOXES  #################################

        $AllServersListBox = new-object System.Windows.Forms.ListBox
        $AllServersListBox.Location = new-object System.Drawing.Size(10,320)
        $AllServersListBox.Size = new-object System.Drawing.Size(140,120)
        $DomainBuildoutForm.Controls.Add($AllServersListBox)


        $DomainControllersListBox = new-object System.Windows.Forms.ListBox
        $DomainControllersListBox.Location = new-object System.Drawing.Size(240,320)
        $DomainControllersListBox.Size = new-object System.Drawing.Size(140,120)
        $DomainBuildoutForm.Controls.Add($DomainControllersListBox)


        $IPSchemaListBox = new-object System.Windows.Forms.ListBox
        $IPSchemaListBox.Location = new-object System.Drawing.Size(10,145)
        $IPSchemaListBox.Size = new-object System.Drawing.Size(135,145)
            $IPSchemaListBox.add_SelectedIndexChanged({
            
            UpdateTextBoxes            
            
            })
        $DomainBuildoutForm.Controls.Add($IPSchemaListBox)
        
        PopulateUsernameAndPassword

        foreach($DomainServer in ($Global:AllCreatedServers | sort)) {
        
        $IPSchemaListBox.Items.Add($DomainServer)
        $AllServersListBox.Items.Add($DomainServer)
        
        }



        #################################  Tool Tips  #################################
                	
        $UsernameToolTip = New-Object System.Windows.Forms.ToolTip
        $UsernameToolTip.SetToolTip($LocalUsernameTextBox, "This is the username input when creating the custom ISO. It's important `nthis is the same across all ISOs created if using multiple.")
        $UsernameToolTip.AutoPopDelay = 10000
        $UsernameToolTip.InitialDelay = 100
        $UsernameToolTip.ToolTipTitle = "Username Help"
        $UsernameToolTip.ToolTipIcon = "Info"
        $UsernameToolTip.IsBalloon = $True


        $PasswordToolTip = New-Object System.Windows.Forms.ToolTip
        $PasswordToolTip.SetToolTip($LocalPasswordTextBox, "This is the password input when creating the custom ISO. It's important `nthis is the same across all ISOs created if using multiple.")
        $PasswordToolTip.AutoPopDelay = 10000
        $PasswordToolTip.InitialDelay = 100
        $PasswordToolTip.ToolTipTitle = "Password Help"
        $PasswordToolTip.ToolTipIcon = "Info"
        $PasswordToolTip.IsBalloon = $True


$DomainBuildoutForm.ShowDialog()

#################################  END FORM  #################################

}


Function UserGroupOUForm {

#################################  (USER - GROUP - OU) FORM  #################################

$Global:BaseDN = ""

    foreach($Split in $DomainNameTextBox.Text.Split(".")) {

    $Global:BaseDN += "DC=$Split,"

    }

$Global:BaseDN = $Global:BaseDN.Remove($Global:BaseDN.Length-1)

$UserGroupOUForm = New-Object system.Windows.Forms.Form
$UserGroupOUForm.Text = "AXL - User, Group, OU Buildout"
$UserGroupOUForm.ClientSize = new-object System.Drawing.Size(390,610)
$UserGroupOUForm.FormBorderStyle = 'Fixed3D'
$UserGroupOUForm.MaximizeBox = $False
$UserGroupOUForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $OUSectionLabel = New-Object System.Windows.Forms.Label
        $OUSectionLabel.Text = "================== Organizational Units =================="
        $OUSectionLabel.Size = new-object System.Drawing.Size(370,20)
        $OUSectionLabel.TextAlign = "MiddleCenter"
        $OUSectionLabel.Location = new-object System.Drawing.Size(10,10)
        $UserGroupOUForm.Controls.Add($OUSectionLabel)


        $OUStructureLabel = New-Object System.Windows.Forms.Label
        $OUStructureLabel.Text = "OU Structure"
        $OUStructureLabel.Size = new-object System.Drawing.Size(210,15)
        $OUStructureLabel.TextAlign = "MiddleCenter"
        $OUStructureLabel.Location = new-object System.Drawing.Size(10,35)
        $UserGroupOUForm.Controls.Add($OUStructureLabel)


        $OUNameLabel = New-Object System.Windows.Forms.Label
        $OUNameLabel.Text = "Input OU Name:"
        $OUNameLabel.AutoSize = $True
        $OUNameLabel.Location = new-object System.Drawing.Size(230,35)
        $UserGroupOUForm.Controls.Add($OUNameLabel)


        $OUNameLabel = New-Object System.Windows.Forms.Label
        $OUNameLabel.Text = "Parent Object:"
        $OUNameLabel.AutoSize = $True
        $OUNameLabel.Location = new-object System.Drawing.Size(230,85)
        $UserGroupOUForm.Controls.Add($OUNameLabel)


        $GroupSectionLabel = New-Object System.Windows.Forms.Label
        $GroupSectionLabel.Text = "======================== Groups ========================"
        $GroupSectionLabel.Size = new-object System.Drawing.Size(370,20)
        $GroupSectionLabel.TextAlign = "MiddleCenter"
        $GroupSectionLabel.Location = new-object System.Drawing.Size(10,205)
        $UserGroupOUForm.Controls.Add($GroupSectionLabel)


        $GroupsLabel = New-Object System.Windows.Forms.Label
        $GroupsLabel.Text = "Groups"
        $GroupsLabel.Size = new-object System.Drawing.Size(120,15)
        $GroupsLabel.TextAlign = "MiddleCenter"
        $GroupsLabel.Location = new-object System.Drawing.Size(10,230)
        $UserGroupOUForm.Controls.Add($GroupsLabel)


        $GroupNameLabel = New-Object System.Windows.Forms.Label
        $GroupNameLabel.Text = "Group Name:"
        $GroupNameLabel.AutoSize = $True
        $GroupNameLabel.Location = new-object System.Drawing.Size(140,230)
        $UserGroupOUForm.Controls.Add($GroupNameLabel)


        $GroupOULabel = New-Object System.Windows.Forms.Label
        $GroupOULabel.Text = "Group OU Location:"
        $GroupOULabel.AutoSize = $True
        $GroupOULabel.Location = new-object System.Drawing.Size(265,230)
        $UserGroupOUForm.Controls.Add($GroupOULabel)


        $GroupScopeLabel = New-Object System.Windows.Forms.Label
        $GroupScopeLabel.Text = "Group Scope:"
        $GroupScopeLabel.AutoSize = $True
        $GroupScopeLabel.Location = new-object System.Drawing.Size(140,280)
        $UserGroupOUForm.Controls.Add($GroupScopeLabel)


        $GroupCategoryLabel = New-Object System.Windows.Forms.Label
        $GroupCategoryLabel.Text = "Group Category:"
        $GroupCategoryLabel.AutoSize = $True
        $GroupCategoryLabel.Location = new-object System.Drawing.Size(265,280)
        $UserGroupOUForm.Controls.Add($GroupCategoryLabel)


        $UserSectionLabel = New-Object System.Windows.Forms.Label
        $UserSectionLabel.Text = "======================== Users ========================"
        $UserSectionLabel.Size = new-object System.Drawing.Size(370,20)
        $UserSectionLabel.TextAlign = "MiddleCenter"
        $UserSectionLabel.Location = new-object System.Drawing.Size(10,370)
        $UserGroupOUForm.Controls.Add($UserSectionLabel)


        $UsersLabel = New-Object System.Windows.Forms.Label
        $UsersLabel.Text = "Users"
        $UsersLabel.Size = new-object System.Drawing.Size(120,15)
        $UsersLabel.TextAlign = "MiddleCenter"
        $UsersLabel.Location = new-object System.Drawing.Size(10,390)
        $UserGroupOUForm.Controls.Add($UsersLabel)


        $FirstNameLabel = New-Object System.Windows.Forms.Label
        $FirstNameLabel.Text = "First Name:"
        $FirstNameLabel.AutoSize = $True
        $FirstNameLabel.Location = new-object System.Drawing.Size(140,390)
        $UserGroupOUForm.Controls.Add($FirstNameLabel)


        $LastNameLabel = New-Object System.Windows.Forms.Label
        $LastNameLabel.Text = "Last Name:"
        $LastNameLabel.AutoSize = $True
        $LastNameLabel.Location = new-object System.Drawing.Size(265,390)
        $UserGroupOUForm.Controls.Add($LastNameLabel)

        
        $LoginNameLabel = New-Object System.Windows.Forms.Label
        $LoginNameLabel.Text = "Login Name:"
        $LoginNameLabel.AutoSize = $True
        $LoginNameLabel.Location = new-object System.Drawing.Size(140,440)
        $UserGroupOUForm.Controls.Add($LoginNameLabel)


        $UserPasswordLabel = New-Object System.Windows.Forms.Label
        $UserPasswordLabel.Text = "Password:"
        $UserPasswordLabel.AutoSize = $True
        $UserPasswordLabel.Location = new-object System.Drawing.Size(265,440)
        $UserGroupOUForm.Controls.Add($UserPasswordLabel)


        $UserOULabel = New-Object System.Windows.Forms.Label
        $UserOULabel.Text = "User OU Location:"
        $UserOULabel.AutoSize = $True
        $UserOULabel.Location = new-object System.Drawing.Size(140,490)
        $UserGroupOUForm.Controls.Add($UserOULabel)



        #################################  TEXT BOXES  #################################

        $OUNameTextBox = new-object System.Windows.Forms.TextBox
        $OUNameTextBox.Location = new-object System.Drawing.Size(230,55)
        $OUNameTextBox.Size = new-object System.Drawing.Size(150,20)
            $OUNameTextBox.Add_TextChanged({
        
                if($OUNameTextBox.Text.Length -lt 1) {
                
                $AddOUButton.Enabled = $False
                
                }

                else {
                
                $AddOUButton.Enabled = $True

                }
        
            })
        $UserGroupOUForm.Controls.Add($OUNameTextBox)


        $GroupNameTextBox = new-object System.Windows.Forms.TextBox
        $GroupNameTextBox.Location = new-object System.Drawing.Size(140,250)
        $GroupNameTextBox.Size = new-object System.Drawing.Size(115,20)
            $GroupNameTextBox.Add_TextChanged({
            
                if($GroupsListBox.SelectedItem -and $GroupNameTextBox.Text.Length -ge 1) {

                $Global:GroupName[$GroupsListBox.SelectedIndex] = $GroupNameTextBox.Text
                $GroupsListBox.Items[$GroupsListBox.SelectedIndex] = $GroupNameTextBox.Text

                }
                        
            })
        $UserGroupOUForm.Controls.Add($GroupNameTextBox)


        $FirstNameTextBox = new-object System.Windows.Forms.TextBox
        $FirstNameTextBox.Location = new-object System.Drawing.Size(140,410)
        $FirstNameTextBox.Size = new-object System.Drawing.Size(115,20)
            $FirstNameTextBox.Add_TextChanged({
            
                if($UsersListBox.SelectedItem -and $FirstNameTextBox.Text.Length -ge 1) {

                $Global:UserFirstName[$UsersListBox.SelectedIndex] = $FirstNameTextBox.Text
                $UsersListBox.Items[$UsersListBox.SelectedIndex] = "$($FirstNameTextBox.Text) $($LastNameTextBox.Text)"

                }
                        
            })
        $UserGroupOUForm.Controls.Add($FirstNameTextBox)
        

        $LastNameTextBox = new-object System.Windows.Forms.TextBox
        $LastNameTextBox.Location = new-object System.Drawing.Size(265,410)
        $LastNameTextBox.Size = new-object System.Drawing.Size(115,20)
            $LastNameTextBox.Add_TextChanged({
            
                if($UsersListBox.SelectedItem -and $LastNameTextBox.Text.Length -ge 1) {

                $Global:UserLastName[$UsersListBox.SelectedIndex] = $LastNameTextBox.Text
                $UsersListBox.Items[$UsersListBox.SelectedIndex] = "$($FirstNameTextBox.Text) $($LastNameTextBox.Text)"

                }
                        
            })
        $UserGroupOUForm.Controls.Add($LastNameTextBox)


        $LoginNameTextBox = new-object System.Windows.Forms.TextBox
        $LoginNameTextBox.Location = new-object System.Drawing.Size(140,460)
        $LoginNameTextBox.Size = new-object System.Drawing.Size(115,20)
            $LoginNameTextBox.Add_TextChanged({
        
                if($UsersListBox.SelectedItem -and $LoginNameTextBox.Text.Length -ge 1) {

                $Global:UserLoginName[$UsersListBox.SelectedIndex] = $LoginNameTextBox.Text

                }        
        
            })
        $UserGroupOUForm.Controls.Add($LoginNameTextBox)


        $UserPasswordTextBox = new-object System.Windows.Forms.TextBox
        $UserPasswordTextBox.Location = new-object System.Drawing.Size(265,460)
        $UserPasswordTextBox.Size = new-object System.Drawing.Size(115,20)
        $UserPasswordTextBox.PasswordChar = '*'
            $UserPasswordTextBox.Add_TextChanged({
            
                if($UsersListBox.SelectedItem -and $UserPasswordTextBox.Text.Length -ge 1) {

                $Global:UserPassword[$UsersListBox.SelectedIndex] = $UserPasswordTextBox.Text

                }
                        
            })
        $UserGroupOUForm.Controls.Add($UserPasswordTextBox)



        #################################  BUTTONS  #################################

        $AddOUButton = new-object System.Windows.Forms.Button
        $AddOUButton.Location = new-object System.Drawing.Size(260,145)
        $AddOUButton.Size = new-object System.Drawing.Size(90,20)
        $AddOUButton.Text = "Add OU"
        $AddOUButton.Enabled = $False
            $AddOUButton.Add_Click({

            AddOU
            $OUNameTextBox.Clear()
            PopulateOUDropDowns

            })
        $UserGroupOUForm.Controls.Add($AddOUButton) 


        $RemoveOUButton = new-object System.Windows.Forms.Button
        $RemoveOUButton.Location = new-object System.Drawing.Size(260,180)
        $RemoveOUButton.Size = new-object System.Drawing.Size(90,20)
        $RemoveOUButton.Text = "Remove OU"
            $RemoveOUButton.Add_Click({
          
                if($OUStructureListBox.SelectedItem -ne $DomainNameTextBox.Text) {

                $SelectedOU = $DropDownParentOU.Text
                $SelectedLocation = ($Global:OULocation[($OUStructureListBox.SelectedIndex-1)]).Split(",",2)[1]
                $OUsToRemove = @()
                $IndexesToRemove = @()
                
                    foreach($RemovedOU in $Global:OULocation) {

                        if($RemovedOU -match $SelectedOU -and $RemovedOU -match $SelectedLocation) {
                        
                        $OUsToRemove += $RemovedOU
                        $IndexesToRemove += ($Global:OULocation.IndexOf($RemovedOU)+1)

                        }
                    
                    }
                
                    foreach($OUToRemove in $OUsToRemove) {

                    $Global:AllCreatedOUs.RemoveAt(($Global:OULocation.IndexOf($OUToRemove))+1)
                    $Global:OULocation.Remove($OUToRemove)

                    }

                    foreach($IndexToRemove in $IndexesToRemove | sort -Descending) {
                    
                    $OUStructureListBox.Items.RemoveAt($IndexToRemove)
                    
                    }

                $DropDownParentOU.SelectedItem = $DomainNameTextBox.Text

                PopulateOUDropDowns

                }

                elseif($OUStructureListBox.SelectedItem -eq $DomainNameTextBox.Text) {
            
                [System.Windows.MessageBox]::Show("You cannot remove the Domain from the OU structure")
            
                }

            })
        $UserGroupOUForm.Controls.Add($RemoveOUButton) 


        $AddGroupButton = new-object System.Windows.Forms.Button
        $AddGroupButton.Location = new-object System.Drawing.Size(140,340)
        $AddGroupButton.Size = new-object System.Drawing.Size(115,20)
        $AddGroupButton.Text = "Add Group"
            $AddGroupButton.Add_Click({

            AddGroup

            })
        $UserGroupOUForm.Controls.Add($AddGroupButton) 


        $RemoveGroupButton = new-object System.Windows.Forms.Button
        $RemoveGroupButton.Location = new-object System.Drawing.Size(265,340)
        $RemoveGroupButton.Size = new-object System.Drawing.Size(115,20)
        $RemoveGroupButton.Text = "Remove Group"
        $RemoveGroupButton.Enabled = $False
            $RemoveGroupButton.Add_Click({
           
            $Global:GroupName.RemoveAt($GroupsListBox.SelectedIndex)
            $Global:GroupOULocation.RemoveAt($GroupsListBox.SelectedIndex)
            $Global:GroupScope.RemoveAt($GroupsListBox.SelectedIndex)
            $Global:GroupType.RemoveAt($GroupsListBox.SelectedIndex) 
            $GroupsListBox.Items.RemoveAt($GroupsListBox.SelectedIndex)    

            })
        $UserGroupOUForm.Controls.Add($RemoveGroupButton) 


        $DeselectGroupButton = new-object System.Windows.Forms.Button
        $DeselectGroupButton.Location = new-object System.Drawing.Size(10,340)
        $DeselectGroupButton.Size = new-object System.Drawing.Size(120,20)
        $DeselectGroupButton.Text = "Deselect Group"
            $DeselectGroupButton.Add_Click({
          
                if($GroupsListBox.SelectedItem) {

                    $GroupsListBox.ClearSelected()
                    $GroupNameTextBox.Clear()
                    $DropDownGroupOU.ResetText()
                    $DropDownGroupScope.ResetText()
                    $DropDownGroupType.ResetText()

                }

            })
        $UserGroupOUForm.Controls.Add($DeselectGroupButton) 


        $AddUserButton = new-object System.Windows.Forms.Button
        $AddUserButton.Location = new-object System.Drawing.Size(140,540)
        $AddUserButton.Size = new-object System.Drawing.Size(115,20)
        $AddUserButton.Text = "Add User"
            $AddUserButton.Add_Click({
          
            AddUser

            })
        $UserGroupOUForm.Controls.Add($AddUserButton) 


        $RemoveUserButton = new-object System.Windows.Forms.Button
        $RemoveUserButton.Location = new-object System.Drawing.Size(265,540)
        $RemoveUserButton.Size = new-object System.Drawing.Size(115,20)
        $RemoveUserButton.Text = "Remove User"
        $RemoveUserButton.Enabled = $False
            $RemoveUserButton.Add_Click({
          
            $Global:UserLoginName.RemoveAt($UsersListBox.SelectedIndex)
            $Global:UserFirstName.RemoveAt($UsersListBox.SelectedIndex)
            $Global:UserLastName.RemoveAt($UsersListBox.SelectedIndex)
            $Global:UserPassword.RemoveAt($UsersListBox.SelectedIndex) 
            $Global:UserOULocation.RemoveAt($UsersListBox.SelectedIndex) 
            $UsersListBox.Items.RemoveAt($UsersListBox.SelectedIndex) 

            })
        $UserGroupOUForm.Controls.Add($RemoveUserButton) 


        $DeselectUserButton = new-object System.Windows.Forms.Button
        $DeselectUserButton.Location = new-object System.Drawing.Size(10,540)
        $DeselectUserButton.Size = new-object System.Drawing.Size(120,20)
        $DeselectUserButton.Text = "Deselect User"
            $DeselectUserButton.Add_Click({
          
                if($UsersListBox.SelectedItem) {

                    $UsersListBox.ClearSelected()
                    $LoginNameTextBox.Clear()
                    $FirstNameTextBox.Clear()
                    $LastNameTextBox.Clear()
                    $UserPasswordTextBox.Clear()
                    $DropDownUsersOU.ResetText()

                }

            })
        $UserGroupOUForm.Controls.Add($DeselectUserButton) 


        $UGOBuildoutPreviousButton = new-object System.Windows.Forms.Button
        $UGOBuildoutPreviousButton.Location = new-object System.Drawing.Size(10,580)
        $UGOBuildoutPreviousButton.Size = new-object System.Drawing.Size(120,20)
        $UGOBuildoutPreviousButton.Text = "Previous $($Global:FinalForms.IndexOf("UGO")) / $($Global:FinalForms.Count)"
            $UGOBuildoutPreviousButton.Add_Click({
          
            $DomainBuildoutForm.Show()
            $UserGroupOUForm.Hide()

            })
        $UserGroupOUForm.Controls.Add($UGOBuildoutPreviousButton) 


        $UGOBuildoutButton = new-object System.Windows.Forms.Button
        $UGOBuildoutButton.Location = new-object System.Drawing.Size(140,580)
        $UGOBuildoutButton.Size = new-object System.Drawing.Size(115,20)
        
        if($Global:FinalForms.IndexOf("UGO") -eq ($Global:FinalForms.Count - 1)) {

        $UGOBuildoutButton.Text = "Finish"

        }

        else{
        
        $UGOBuildoutButton.Text = "Next $($Global:FinalForms.IndexOf("UGO")+2) / $($Global:FinalForms.Count)"
        
        }

            $UGOBuildoutButton.Add_Click({
          
                if($CertificateServicesCheckbox.CheckState -eq "Checked") {
                    
                $UserGroupOUForm.Hide()
                CertificateBuildoutForm
            
                }

                elseif($SQLCheckbox.CheckState -eq "Checked") {
                    
                $UserGroupOUForm.Hide()
                SQLBuildoutForm
            
                }

                elseif($DFSCheckbox.CheckState -eq "Checked") {
                    
                $UserGroupOUForm.Hide()
                DFSBuildoutForm
            
                }


                else{
                
                $UserGroupOUForm.Hide()
                StatusForm
                
                }

            })
        $UserGroupOUForm.Controls.Add($UGOBuildoutButton) 


        $UGOBuildoutExitButton = new-object System.Windows.Forms.Button
        $UGOBuildoutExitButton.Location = new-object System.Drawing.Size(265,580)
        $UGOBuildoutExitButton.Size = new-object System.Drawing.Size(115,20)
        $UGOBuildoutExitButton.Text = "Exit"
            $UGOBuildoutExitButton.Add_Click({

            DisconnectXenHost
            $UserGroupOUForm.Close()

            })
        $UserGroupOUForm.Controls.Add($UGOBuildoutExitButton) 



        #################################  DROP DOWN BOXES  #################################

        $DropDownParentOU = new-object System.Windows.Forms.ComboBox
        $DropDownParentOU.Location = new-object System.Drawing.Size(230,105)
        $DropDownParentOU.Size = new-object System.Drawing.Size(150,20)
            $DropDownParentOU.Add_SelectedIndexChanged({
            
                if($OUStructureListBox.Items -match $DropDownParentOU.SelectedItem) {
                
                $OUStructureListBox.SelectedItem = $OUStructureListBox.Items | where {$_ -match $DropDownParentOU.SelectedItem}

                }
        
            })
        $UserGroupOUForm.Controls.Add($DropDownParentOU)
        

        $DropDownGroupOU = new-object System.Windows.Forms.ComboBox
        $DropDownGroupOU.Location = new-object System.Drawing.Size(265,250)
        $DropDownGroupOU.Size = new-object System.Drawing.Size(115,20)
            $DropDownGroupOU.Add_SelectedIndexChanged({

                if($GroupsListBox.SelectedItem) {

                $Global:GroupOULocation[$GroupsListBox.SelectedIndex] = $DropDownGroupOU.Text

                }
        
            })
        $UserGroupOUForm.Controls.Add($DropDownGroupOU)


        $DropDownGroupScope = new-object System.Windows.Forms.ComboBox
        $DropDownGroupScope.Location = new-object System.Drawing.Size(140,300)
        $DropDownGroupScope.Size = new-object System.Drawing.Size(115,20)
            $DropDownGroupScope.Add_SelectedIndexChanged({

                if($GroupsListBox.SelectedItem) {

                $Global:GroupScope[$GroupsListBox.SelectedIndex] = $DropDownGroupScope.Text

                }
        
            })
        $UserGroupOUForm.Controls.Add($DropDownGroupScope)


        $DropDownGroupType = new-object System.Windows.Forms.ComboBox
        $DropDownGroupType.Location = new-object System.Drawing.Size(265,300)
        $DropDownGroupType.Size = new-object System.Drawing.Size(115,20)
            $DropDownGroupType.Add_SelectedIndexChanged({

                if($GroupsListBox.SelectedItem) {

                $Global:GroupType[$GroupsListBox.SelectedIndex] = $DropDownGroupType.Text

                }
        
            })
        $UserGroupOUForm.Controls.Add($DropDownGroupType)


        $DropDownUsersOU = new-object System.Windows.Forms.ComboBox
        $DropDownUsersOU.Location = new-object System.Drawing.Size(140,510)
        $DropDownUsersOU.Size = new-object System.Drawing.Size(240,20)
            $DropDownUsersOU.Add_SelectedIndexChanged({

                if($UsersListBox.SelectedItem) {

                $Global:UserOULocation[$UsersListBox.SelectedIndex] = $DropDownUsersOU.Text

                }
        
            })
        $UserGroupOUForm.Controls.Add($DropDownUsersOU)
        
        

        #################################  LIST BOXES  #################################

        $OUStructureListBox = new-object System.Windows.Forms.ListBox
        $OUStructureListBox.Location = new-object System.Drawing.Size(10,55)
        $OUStructureListBox.Size = new-object System.Drawing.Size(210,150)
            $OUStructureListBox.Add_SelectedIndexChanged({
        
            $DropDownParentOU.Text = $OUStructureListBox.Text.Substring($OUStructureListBox.Text.IndexOf("-")+1)      
        
            })
        $UserGroupOUForm.Controls.Add($OUStructureListBox)

        $GroupsListBox = new-object System.Windows.Forms.ListBox
        $GroupsListBox.Location = new-object System.Drawing.Size(10,250)
        $GroupsListBox.Size = new-object System.Drawing.Size(120,90)
            $GroupsListBox.Add_SelectedIndexChanged({

            if($GroupsListBox.SelectedItem) {

            $GroupNameTextBox.Text = $Global:GroupName[$GroupsListBox.SelectedIndex]
            $DropDownGroupOU.Text = $Global:GroupOULocation[$GroupsListBox.SelectedIndex]
            $DropDownGroupScope.Text = $Global:GroupScope[$GroupsListBox.SelectedIndex]
            $DropDownGroupType.Text = $Global:GroupType[$GroupsListBox.SelectedIndex]   
            $RemoveGroupButton.Enabled = $True
            $AddGroupButton.Enabled = $False

            }

            else {
            
            $RemoveGroupButton.Enabled = $False
            $AddGroupButton.Enabled = $True

            }

            })
        $UserGroupOUForm.Controls.Add($GroupsListBox)


        $UsersListBox = new-object System.Windows.Forms.ListBox
        $UsersListBox.Location = new-object System.Drawing.Size(10,410)
        $UsersListBox.Size = new-object System.Drawing.Size(120,130)
            $UsersListBox.Add_SelectedIndexChanged({
                    
                if($UsersListBox.SelectedItem) {

                $LoginNameTextBox.Text = $Global:UserLoginName[$UsersListBox.SelectedIndex]
                $FirstNameTextBox.Text = $Global:UserFirstName[$UsersListBox.SelectedIndex]
                $LastNameTextBox.Text = $Global:UserLastName[$UsersListBox.SelectedIndex]
                $UserPasswordTextBox.Text = $Global:UserPassword[$UsersListBox.SelectedIndex]
                $DropDownUsersOU.Text = $Global:UserOULocation[$UsersListBox.SelectedIndex] 
                $RemoveUserButton.Enabled = $True
                $AddUserButton.Enabled = $False

                }

                else {
                
                $RemoveUserButton.Enabled = $False
                $AddUserButton.Enabled = $True

                }

            })
        $UserGroupOUForm.Controls.Add($UsersListBox)


        if($Global:AllCreatedOUs.Count -lt 1) {

        $OUStructureListBox.Items.Add($DomainNameTextBox.Text)
        $Global:AllCreatedOUs += $DomainNameTextBox.Text
        
        }

        else {

            if($Global:BaseDN -notmatch $Global:OULocation[0].Substring($Global:OULocation[0].IndexOf("DC"))) {

            $Global:AllCreatedOUs.Insert(0,$DomainNameTextBox.Text)
            $Global:AllCreatedOUs.RemoveAt(1)

            $Global:OULocation = $Global:OULocation -replace $Global:OULocation[0].Substring($Global:OULocation[0].IndexOf("DC")),$Global:BaseDN
            
            }

            foreach($OU in $Global:AllCreatedOUs) {
        
                if($OU -ne $Null) {

                $OUStructureListBox.Items.Add($OU)
        
                }

            }

        }
        
        PopulateOUDropDowns
        PopulateGroupDropDowns
        $DropDownParentOU.SelectedItem = $DomainNameTextBox.Text

        if($Global:GroupName.Count -ge 1) {
        
            foreach($Group in $Global:GroupName) {

            $GroupsListBox.Items.Add($Group)

            }
        
        }

        if($Global:UserLoginName.Count -ge 1) {
        
            foreach($User in $Global:UserLoginName) {

            $UsersListBox.Items.Add($User)

            }
        
        }

        
$UserGroupOUForm.ShowDialog()

#################################  END FORM  #################################

}


Function CertificateBuildoutForm {

#################################  CERTIFICATE BUILDOUT FORM  #################################

$CertificateBuildoutForm = New-Object system.Windows.Forms.Form
$CertificateBuildoutForm.Text = "AXL - Certificate Services Buildout"
$CertificateBuildoutForm.ClientSize = new-object System.Drawing.Size(390,360)
$CertificateBuildoutForm.FormBorderStyle = 'Fixed3D'
$CertificateBuildoutForm.MaximizeBox = $False
$CertificateBuildoutForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $AllMembersLabel = New-Object System.Windows.Forms.Label
        $AllMembersLabel.Text = "All Members"
        $AllMembersLabel.Size = new-object System.Drawing.Size(140,20)
        $AllMembersLabel.Location = new-object System.Drawing.Size(10,10)
        $AllMembersLabel.TextAlign = "MiddleCenter"
        $CertificateBuildoutForm.Controls.Add($AllMembersLabel) 


        $CertificateAuthoritiesLabel = New-Object System.Windows.Forms.Label
        $CertificateAuthoritiesLabel.Text = "Certificate Authorities"
        $CertificateAuthoritiesLabel.Size = new-object System.Drawing.Size(140,20)
        $CertificateAuthoritiesLabel.Location = new-object System.Drawing.Size(240,10)
        $CertificateAuthoritiesLabel.TextAlign = "MiddleCenter"
        $CertificateBuildoutForm.Controls.Add($CertificateAuthoritiesLabel) 


        $CANameLabel = New-Object System.Windows.Forms.Label
        $CANameLabel.Text = "CA Name:"
        $CANameLabel.AutoSize = $True
        $CANameLabel.Location = new-object System.Drawing.Size(10,150)
        $CertificateBuildoutForm.Controls.Add($CANameLabel) 


        $CATypeLabel = New-Object System.Windows.Forms.Label
        $CATypeLabel.Text = "CA Type:"
        $CATypeLabel.AutoSize = $True
        $CATypeLabel.Location = new-object System.Drawing.Size(140,150)
        $CertificateBuildoutForm.Controls.Add($CATypeLabel) 


        $ParentCALabel = New-Object System.Windows.Forms.Label
        $ParentCALabel.Text = "Parent CA:"
        $ParentCALabel.AutoSize = $True
        $ParentCALabel.Location = new-object System.Drawing.Size(290,150)
        $CertificateBuildoutForm.Controls.Add($ParentCALabel)


        $CryptoProviderLabel = New-Object System.Windows.Forms.Label
        $CryptoProviderLabel.Text = "Cryptographic Provider:"
        $CryptoProviderLabel.AutoSize = $True
        $CryptoProviderLabel.Location = new-object System.Drawing.Size(10,200)
        $CertificateBuildoutForm.Controls.Add($CryptoProviderLabel) 


        $KeyLengthLabel = New-Object System.Windows.Forms.Label
        $KeyLengthLabel.Text = "Key Length:"
        $KeyLengthLabel.AutoSize = $True
        $KeyLengthLabel.Location = new-object System.Drawing.Size(310,200)
        $CertificateBuildoutForm.Controls.Add($KeyLengthLabel) 


        $ValidityPeriodLabel = New-Object System.Windows.Forms.Label
        $ValidityPeriodLabel.Text = "Certificate Validity Period:"
        $ValidityPeriodLabel.AutoSize = $True
        $ValidityPeriodLabel.Location = new-object System.Drawing.Size(10,250)
        $CertificateBuildoutForm.Controls.Add($ValidityPeriodLabel) 


        $ValidityUnitsLabel = New-Object System.Windows.Forms.Label
        $ValidityUnitsLabel.Text = "Validity Length:"
        $ValidityUnitsLabel.AutoSize = $True
        $ValidityUnitsLabel.Location = new-object System.Drawing.Size(160,250)
        $CertificateBuildoutForm.Controls.Add($ValidityUnitsLabel) 


        $HashAlgorithmLabel = New-Object System.Windows.Forms.Label
        $HashAlgorithmLabel.Text = "Hash Algorithm:"
        $HashAlgorithmLabel.AutoSize = $True
        $HashAlgorithmLabel.Location = new-object System.Drawing.Size(280,250)
        $CertificateBuildoutForm.Controls.Add($HashAlgorithmLabel) 
             


        #################################  TEXT BOXES  #################################

        $CANameTextBox = new-object System.Windows.Forms.TextBox
        $CANameTextBox.Location = new-object System.Drawing.Size(10,170)
        $CANameTextBox.Size = new-object System.Drawing.Size(120,20)
        $CANameTextBox.Enabled = $False
            $CANameTextBox.Add_TextChanged({

                if($CANameTextBox.Text -ne $null) {

                $Global:CANames[$CertificateAuthoritiesListBox.SelectedIndex] = $CANameTextBox.Text

                }
            
            $CertificateBuildoutButton.Enabled = $False

            })
        $CertificateBuildoutForm.Controls.Add($CANameTextBox)



        #################################  BUTTONS  #################################

        $CertificatePreviousButton = new-object System.Windows.Forms.Button
        $CertificatePreviousButton.Location = new-object System.Drawing.Size(10,330)
        $CertificatePreviousButton.Size = new-object System.Drawing.Size(90,20)
        $CertificatePreviousButton.Text = "Previous $($Global:FinalForms.IndexOf("Certificate")) / $($Global:FinalForms.Count)"
            $CertificatePreviousButton.Add_Click({
          
            $UserGroupOUForm.Show()
            $CertificateBuildoutForm.Hide()         

            })
        $CertificateBuildoutForm.Controls.Add($CertificatePreviousButton) 
        

        $CertificateBuildoutButton = new-object System.Windows.Forms.Button
        $CertificateBuildoutButton.Location = new-object System.Drawing.Size(110,330)
        $CertificateBuildoutButton.Size = new-object System.Drawing.Size(80,20)
        
        if($Global:FinalForms.IndexOf("Certificate") -eq ($Global:FinalForms.Count - 1)) {

        $CertificateBuildoutButton.Text = "Finish"

        }

        else{
        
        $CertificateBuildoutButton.Text = "Next $($Global:FinalForms.IndexOf("Certificate")+2) / $($Global:FinalForms.Count)"
        
        }

        $CertificateBuildoutButton.Enabled = $False
            $CertificateBuildoutButton.Add_Click({
          
                if($Global:FinalForms.IndexOf("Certificate") -ne ($Global:FinalForms.Count - 1)){

                    if($SQLCheckbox.CheckState -eq "Checked") {
                    
                    $CertificateBuildoutForm.Hide()
                    SQLBuildoutForm
            
                    }

                    elseif($DFSCheckbox.CheckState -eq "Checked") {
                    
                    $CertificateBuildoutForm.Hide()
                    DFSBuildoutForm
            
                    }

                }

                else{
                
                $CertificateBuildoutForm.Hide()
                StatusForm
                
                }

            })
        $CertificateBuildoutForm.Controls.Add($CertificateBuildoutButton) 


        $CertificateBuildoutValidateButton = new-object System.Windows.Forms.Button
        $CertificateBuildoutValidateButton.Location = new-object System.Drawing.Size(200,330)
        $CertificateBuildoutValidateButton.Size = new-object System.Drawing.Size(80,20)
        $CertificateBuildoutValidateButton.Text = "Validate"
            $CertificateBuildoutValidateButton.Add_Click({

            ValidateCertificateCreation

            })
        $CertificateBuildoutForm.Controls.Add($CertificateBuildoutValidateButton) 


        $CertificateBuildoutNextButton = new-object System.Windows.Forms.Button
        $CertificateBuildoutNextButton.Location = new-object System.Drawing.Size(290,330)
        $CertificateBuildoutNextButton.Size = new-object System.Drawing.Size(90,20)
        $CertificateBuildoutNextButton.Text = "Exit"
            $CertificateBuildoutNextButton.Add_Click({

            DisconnectXenHost
            $CertificateBuildoutForm.Close()

            })
        $CertificateBuildoutForm.Controls.Add($CertificateBuildoutNextButton)
        

        $AddServerButton = new-object System.Windows.Forms.Button
        $AddServerButton.Location = new-object System.Drawing.Size(175,60)
        $AddServerButton.Size = new-object System.Drawing.Size(40,20)
        $AddServerButton.Text = ">>>"
            $AddServerButton.Add_Click({

                if($AllCertificateServersListBox.SelectedItem){
            
                $SelectedItem = $AllCertificateServersListBox.SelectedItem
                $CertificateAuthoritiesListBox.Items.Add($AllCertificateServersListBox.SelectedItem)
                $AllCertificateServersListBox.Items.Remove($AllCertificateServersListBox.SelectedItem)
                $AllCertificateServersListBox.ClearSelected()
                $CertificateAuthoritiesListBox.ClearSelected()

                $PrimaryCAListedServers = @()
                $Counter = 0

                    while($Counter -lt $CertificateAuthoritiesListBox.Items.Count) {
                
                    $PrimaryCAListedServers += $CertificateAuthoritiesListBox.Items[$Counter]

                    $Counter++

                    }

                    $CertificateAuthoritiesListBox.Items.Clear()

                    foreach($PrimaryCAListedServer in ($PrimaryCAListedServers | Sort)) {
                    
                    $CertificateAuthoritiesListBox.Items.Add($PrimaryCAListedServer)

                    }

                    while($Global:CACounter -lt $CertificateAuthoritiesListBox.Items.Count) {
    
                    $Global:CANames += $Null
                    $Global:CAWebEnrollment += $Null
                    $Global:CAResponder += $Null
                    $Global:CATypes += $Null
                    $Global:ParentCA += $Null
                    $Global:CAHashAlgorithm += $Null
                    $Global:CACryptoProvider += $Null
                    $Global:CAKeyLength += $Null
                    $Global:CAValidityPeriod += $Null
                    $Global:CAValidityPeriodUnits += $Null
                    $Global:CACounter++

                    }

                $CertificateBuildoutButton.Enabled = $False
                $Global:CAServers.Insert($CertificateAuthoritiesListBox.Items.IndexOf($SelectedItem),$SelectedItem)
                FillCertificateDropDowns

                }

            })
        $CertificateBuildoutForm.Controls.Add($AddServerButton) 


        $RemoveServerButton = new-object System.Windows.Forms.Button
        $RemoveServerButton.Location = new-object System.Drawing.Size(175,90)
        $RemoveServerButton.Size = new-object System.Drawing.Size(40,20)
        $RemoveServerButton.Text = "<<<"
            $RemoveServerButton.Add_Click({
            
                if($CertificateAuthoritiesListBox.SelectedItem){

                $CASelectedIndex = $CertificateAuthoritiesListBox.SelectedIndex
            
                    if($CertificateAuthoritiesListBox.Text -match [regex]"\*") {

                    $AllCertificateServersListBox.Items.Add(($CertificateAuthoritiesListBox.Text).Replace("*",""))
                    $CertificateAuthoritiesListBox.Items.Remove($CertificateAuthoritiesListBox.Text)

                    }

                    else {
                    
                    $AllCertificateServersListBox.Items.Add($CertificateAuthoritiesListBox.SelectedItem)
                    $CertificateAuthoritiesListBox.Items.Remove($CertificateAuthoritiesListBox.SelectedItem)                 
                    
                    }

                $Global:CAServers.RemoveAt($CASelectedIndex)
                $AllCertificateServersListBox.ClearSelected()
                $CertificateAuthoritiesListBox.ClearSelected()

                $ListedServers = @()
                
                    while($ListedServers.Count -lt $AllCertificateServersListBox.Items.Count) {
                
                    $ListedServers += $AllCertificateServersListBox.Items[$Counter]
                    
                    }

                    $AllCertificateServersListBox.Items.Clear()
                    
                    foreach($ListedServer in ($ListedServers | Sort)) {
                    
                    $AllCertificateServersListBox.Items.Add($ListedServer)

                    }

                $Global:CANames.RemoveAt($CASelectedIndex)
                $Global:CAWebEnrollment.RemoveAt($CASelectedIndex)
                $Global:CAResponder.RemoveAt($CASelectedIndex)
                $Global:CATypes.RemoveAt($CASelectedIndex)
                $Global:ParentCA.RemoveAt($CASelectedIndex)
                $Global:CAHashAlgorithm.RemoveAt($CASelectedIndex)
                $Global:CACryptoProvider.RemoveAt($CASelectedIndex)
                $Global:CAKeyLength.RemoveAt($CASelectedIndex)
                $Global:CAValidityPeriod.RemoveAt($CASelectedIndex)
                $Global:CAValidityPeriodUnits.RemoveAt($CASelectedIndex)

                $Global:CACounter--
                $CertificateBuildoutButton.Enabled = $False
   
                }

            })
        $CertificateBuildoutForm.Controls.Add($RemoveServerButton) 


        
        #################################  DROP DOWN BOXES  #################################

        $DropDownCAType = new-object System.Windows.Forms.ComboBox
        $DropDownCAType.Location = new-object System.Drawing.Size(140,170)
        $DropDownCAType.Size = new-object System.Drawing.Size(140,20)
        $DropDownCAType.Enabled = $False
            $DropDownCAType.Add_SelectedIndexChanged({

                if($DropDownCAType.Text) {

                $Global:CATypes[$CertificateAuthoritiesListBox.SelectedIndex] = $DropDownCAType.Text

                }

                if($Global:CATypes[$CertificateAuthoritiesListBox.SelectedIndex] -match "Subordinate") {
                
                    if($DropDownParentCA.Items -contains $CertificateAuthoritiesListBox.SelectedItem) {

                    $DropDownParentCA.Items.Remove($CertificateAuthoritiesListBox.SelectedItem)

                    }

                $DropDownParentCA.Enabled = $True
                $DropDownCryptoProvider.Enabled = $False
                $DropDownKeyLength.Enabled = $False
                $DropDownValidityPeriod.Enabled = $False
                $DropDownValidityPeriodUnits.Enabled = $False
                $DropDownHashAlgorithm.Enabled = $False
                $DropDownCryptoProvider.ResetText()
                $DropDownKeyLength.ResetText()
                $DropDownValidityPeriod.ResetText()
                $DropDownValidityPeriodUnits.ResetText()
                $DropDownHashAlgorithm.ResetText()

                }

                else {

                    if($DropDownParentCA.Items -notcontains $CertificateAuthoritiesListBox.SelectedItem) {

                    $DropDownParentCA.Items.Add($CertificateAuthoritiesListBox.SelectedItem)

                    }

                $DropDownParentCA.ResetText()
                $DropDownCryptoProvider.Enabled = $True
                $DropDownKeyLength.Enabled = $True
                $DropDownValidityPeriod.Enabled = $True
                $DropDownValidityPeriodUnits.Enabled = $True
                $DropDownHashAlgorithm.Enabled = $True
                $Global:ParentCA[$CertificateAuthoritiesListBox.SelectedIndex] = $Null
                $DropDownParentCA.Enabled = $False
                $DropDownHashAlgorithm.Text = $Global:CAHashAlgorithm[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownParentCA.Text = $Global:ParentCA[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownCryptoProvider.Text = $Global:CACryptoProvider[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownKeyLength.Text = $Global:CAKeyLength[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownValidityPeriod.Text = $Global:CAValidityPeriod[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownValidityPeriodUnits.Text = $Global:CAValidityPeriodUnits[$CertificateAuthoritiesListBox.SelectedIndex]

                }
            
            $CertificateBuildoutButton.Enabled = $False

            })
        $CertificateBuildoutForm.Controls.Add($DropDownCAType)


        $DropDownParentCA = new-object System.Windows.Forms.ComboBox
        $DropDownParentCA.Location = new-object System.Drawing.Size(290,170)
        $DropDownParentCA.Size = new-object System.Drawing.Size(90,20)
        $DropDownParentCA.Enabled = $False
            $DropDownParentCA.Add_SelectedIndexChanged({

            $Global:ParentCA[$CertificateAuthoritiesListBox.SelectedIndex] = $DropDownParentCA.Text
            
            $CertificateBuildoutButton.Enabled = $False

            })
        $CertificateBuildoutForm.Controls.Add($DropDownParentCA)
        

        $DropDownCryptoProvider = new-object System.Windows.Forms.ComboBox
        $DropDownCryptoProvider.Location = new-object System.Drawing.Size(10,220)
        $DropDownCryptoProvider.Size = new-object System.Drawing.Size(290,20)
            $DropDownCryptoProvider.Add_SelectedIndexChanged({

            $Global:CACryptoProvider[$CertificateAuthoritiesListBox.SelectedIndex] = $DropDownCryptoProvider.Text

            $CertificateBuildoutButton.Enabled = $False

            })
        $CertificateBuildoutForm.Controls.Add($DropDownCryptoProvider)


        $DropDownKeyLength = new-object System.Windows.Forms.ComboBox
        $DropDownKeyLength.Location = new-object System.Drawing.Size(310,220)
        $DropDownKeyLength.Size = new-object System.Drawing.Size(70,20)
            $DropDownKeyLength.Add_SelectedIndexChanged({

            $Global:CAKeyLength[$CertificateAuthoritiesListBox.SelectedIndex] = $DropDownKeyLength.Text

            $CertificateBuildoutButton.Enabled = $False

            })
        $CertificateBuildoutForm.Controls.Add($DropDownKeyLength)


        $DropDownValidityPeriod = new-object System.Windows.Forms.ComboBox
        $DropDownValidityPeriod.Location = new-object System.Drawing.Size(10,270)
        $DropDownValidityPeriod.Size = new-object System.Drawing.Size(140,20)
            $DropDownValidityPeriod.Add_SelectedIndexChanged({

            $Global:CAValidityPeriod[$CertificateAuthoritiesListBox.SelectedIndex] = $DropDownValidityPeriod.Text

            $CertificateBuildoutButton.Enabled = $False

            })
        $CertificateBuildoutForm.Controls.Add($DropDownValidityPeriod)


        $DropDownValidityPeriodUnits = new-object System.Windows.Forms.ComboBox
        $DropDownValidityPeriodUnits.Location = new-object System.Drawing.Size(160,270)
        $DropDownValidityPeriodUnits.Size = new-object System.Drawing.Size(110,20)
            $DropDownValidityPeriodUnits.Add_SelectedIndexChanged({

            $Global:CAValidityPeriodUnits[$CertificateAuthoritiesListBox.SelectedIndex] = $DropDownValidityPeriodUnits.Text

            $CertificateBuildoutButton.Enabled = $False

            })
        $CertificateBuildoutForm.Controls.Add($DropDownValidityPeriodUnits)


        $DropDownHashAlgorithm = new-object System.Windows.Forms.ComboBox
        $DropDownHashAlgorithm.Location = new-object System.Drawing.Size(280,270)
        $DropDownHashAlgorithm.Size = new-object System.Drawing.Size(100,20)
            $DropDownHashAlgorithm.Add_SelectedIndexChanged({

            $Global:CAHashAlgorithm[$CertificateAuthoritiesListBox.SelectedIndex] = $DropDownHashAlgorithm.Text

            $CertificateBuildoutButton.Enabled = $False

            })
        $CertificateBuildoutForm.Controls.Add($DropDownHashAlgorithm)



        #################################  LIST BOXES  #################################

        $AllCertificateServersListBox = new-object System.Windows.Forms.ListBox
        $AllCertificateServersListBox.Location = new-object System.Drawing.Size(10,30)
        $AllCertificateServersListBox.Size = new-object System.Drawing.Size(140,120)
        $CertificateBuildoutForm.Controls.Add($AllCertificateServersListBox)

        
        $CertificateAuthoritiesListBox = new-object System.Windows.Forms.ListBox
        $CertificateAuthoritiesListBox.Location = new-object System.Drawing.Size(240,30)
        $CertificateAuthoritiesListBox.Size = new-object System.Drawing.Size(140,120)
            $CertificateAuthoritiesListBox.Add_SelectedIndexChanged({
            
                if($CertificateAuthoritiesListBox.Items.Count -ge 1) {
                
                $DropDownCAType.Enabled = $True
                $CANameTextBox.Enabled = $True
                $WebEnrollmentCheckbox.Enabled = $True
                $OnlineResponderCheckbox.Enabled = $True                
   
                }

                else {
                
                $DropDownCAType.Enabled = $False
                $CANameTextBox.Enabled = $False
                $WebEnrollmentCheckbox.Enabled = $False
                $OnlineResponderCheckbox.Enabled = $False 
                
                }

                if($Global:CATypes[$CertificateAuthoritiesListBox.SelectedIndex] -match "Subordinate") {

                $CANameTextBox.Text = $Global:CANames[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownCAType.Text = $Global:CATypes[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownParentCA.Text = $Global:ParentCA[$CertificateAuthoritiesListBox.SelectedIndex]                
                $DropDownParentCA.Enabled = $True
                $DropDownCryptoProvider.Enabled = $False
                $DropDownKeyLength.Enabled = $False
                $DropDownValidityPeriod.Enabled = $False
                $DropDownValidityPeriodUnits.Enabled = $False
                $DropDownHashAlgorithm.Enabled = $False
                $DropDownCryptoProvider.ResetText()
                $DropDownKeyLength.ResetText()
                $DropDownValidityPeriod.ResetText()
                $DropDownValidityPeriodUnits.ResetText()
                $DropDownHashAlgorithm.ResetText()

                }

                else{
                
                $CANameTextBox.Text = $Global:CANames[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownCAType.Text = $Global:CATypes[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownHashAlgorithm.Text = $Global:CAHashAlgorithm[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownParentCA.Text = $Global:ParentCA[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownCryptoProvider.Text = $Global:CACryptoProvider[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownKeyLength.Text = $Global:CAKeyLength[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownValidityPeriod.Text = $Global:CAValidityPeriod[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownValidityPeriodUnits.Text = $Global:CAValidityPeriodUnits[$CertificateAuthoritiesListBox.SelectedIndex]
                $DropDownParentCA.Enabled = $False
                $DropDownCryptoProvider.Enabled = $True
                $DropDownKeyLength.Enabled = $True
                $DropDownValidityPeriod.Enabled = $True
                $DropDownValidityPeriodUnits.Enabled = $True
                $DropDownHashAlgorithm.Enabled = $True
                
                }

                if($Global:CAWebEnrollment[$CertificateAuthoritiesListBox.SelectedIndex]) {
                
                $WebEnrollmentCheckbox.CheckState = $Global:CAWebEnrollment[$CertificateAuthoritiesListBox.SelectedIndex]
                
                }

                else {
                
                $WebEnrollmentCheckbox.CheckState = "Unchecked"
                
                }

                if($Global:CAResponder[$CertificateAuthoritiesListBox.SelectedIndex]) {
                
                $OnlineResponderCheckbox.CheckState = $Global:CAResponder[$CertificateAuthoritiesListBox.SelectedIndex]
                
                }

                else {
                
                $OnlineResponderCheckbox.CheckState = "Unchecked"
                
                }
        
            })
        $CertificateBuildoutForm.Controls.Add($CertificateAuthoritiesListBox)

        foreach($DomainServer in ($Global:AllCreatedServers | sort)) {
        
        $AllCertificateServersListBox.Items.Add($DomainServer)
        
        }



        #################################  CHECK BOXES  #################################

        $WebEnrollmentCheckbox = New-Object System.Windows.Forms.Checkbox 
        $WebEnrollmentCheckbox.Location = New-Object System.Drawing.Size(40,300)
        $WebEnrollmentCheckbox.Size = new-object System.Drawing.Size(160,20)
        $WebEnrollmentCheckbox.Text = "AD CS Web Enrollment"
        $WebEnrollmentCheckbox.Enabled = $False
            $WebEnrollmentCheckbox.Add_CheckStateChanged({

            $Global:CAWebEnrollment[$CertificateAuthoritiesListBox.SelectedIndex] = $WebEnrollmentCheckbox.CheckState

            })
        $CertificateBuildoutForm.Controls.Add($WebEnrollmentCheckbox)


        $OnlineResponderCheckbox = New-Object System.Windows.Forms.Checkbox 
        $OnlineResponderCheckbox.Location = New-Object System.Drawing.Size(210,300)
        $OnlineResponderCheckbox.Size = new-object System.Drawing.Size(160,20)
        $OnlineResponderCheckbox.Text = "AD CS Online Responder"
        $OnlineResponderCheckbox.Enabled = $False
            $OnlineResponderCheckbox.Add_CheckStateChanged({

            $Global:CAResponder[$CertificateAuthoritiesListBox.SelectedIndex] = $OnlineResponderCheckbox.CheckState

            })
        $CertificateBuildoutForm.Controls.Add($OnlineResponderCheckbox)

    if($Global:CAServers.Count -ge 1) {

    FillCertificateDropDowns
        
        foreach($CAServer in $Global:CAServers) {

        $CertificateAuthoritiesListBox.Items.Add($CAServer)
        $AllCertificateServersListBox.Items.Remove($CAServer)

        }
        
    }

    
$CertificateBuildoutForm.ShowDialog()

#################################  END FORM  #################################

}


Function DFSBuildoutForm {

#################################  DFS BUILDOUT FORM  #################################

$DFSBuildoutForm = New-Object system.Windows.Forms.Form
$DFSBuildoutForm.Text = "AXL - DFS Buildout"
$DFSBuildoutForm.ClientSize = new-object System.Drawing.Size(390,420)
$DFSBuildoutForm.FormBorderStyle = 'Fixed3D'
$DFSBuildoutForm.MaximizeBox = $False
$DFSBuildoutForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $DFSServersLabel = New-Object System.Windows.Forms.Label
        $DFSServersLabel.Text = "All Members"
        $DFSServersLabel.Size = new-object System.Drawing.Size(140,20)
        $DFSServersLabel.Location = new-object System.Drawing.Size(10,10)
        $DFSServersLabel.TextAlign = "MiddleCenter"
        $DFSBuildoutForm.Controls.Add($DFSServersLabel) 


        $DFSServersLabel = New-Object System.Windows.Forms.Label
        $DFSServersLabel.Text = "DFS Servers"
        $DFSServersLabel.Size = new-object System.Drawing.Size(140,20)
        $DFSServersLabel.Location = new-object System.Drawing.Size(240,10)
        $DFSServersLabel.TextAlign = "MiddleCenter"
        $DFSBuildoutForm.Controls.Add($DFSServersLabel) 


        $DFSRootNameLabel = New-Object System.Windows.Forms.Label
        $DFSRootNameLabel.Text = "DFS Root Name:"
        $DFSRootNameLabel.AutoSize = $True
        $DFSRootNameLabel.Location = new-object System.Drawing.Size(10,155)
        $DFSBuildoutForm.Controls.Add($DFSRootNameLabel) 


        $DFSRootsLabel = New-Object System.Windows.Forms.Label
        $DFSRootsLabel.Text = "DFS Roots"
        $DFSRootsLabel.Size = new-object System.Drawing.Size(140,20)
        $DFSRootsLabel.Location = new-object System.Drawing.Size(240,155)
        $DFSRootsLabel.TextAlign = "MiddleCenter"
        $DFSBuildoutForm.Controls.Add($DFSRootsLabel) 


        $DFSFoldersLabel = New-Object System.Windows.Forms.Label
        $DFSFoldersLabel.Text = "DFS Folders"
        $DFSFoldersLabel.Size = new-object System.Drawing.Size(140,20)
        $DFSFoldersLabel.Location = new-object System.Drawing.Size(240,250)
        $DFSFoldersLabel.TextAlign = "MiddleCenter"
        $DFSBuildoutForm.Controls.Add($DFSFoldersLabel) 


        $DFSFolderNameLabel = New-Object System.Windows.Forms.Label
        $DFSFolderNameLabel.Text = "DFS Folder:"
        $DFSFolderNameLabel.AutoSize = $True
        $DFSFolderNameLabel.Location = new-object System.Drawing.Size(10,250)
        $DFSBuildoutForm.Controls.Add($DFSFolderNameLabel) 


        $DFSRootSelectionLabel = New-Object System.Windows.Forms.Label
        $DFSRootSelectionLabel.Text = "DFS Root:"
        $DFSRootSelectionLabel.AutoSize = $True
        $DFSRootSelectionLabel.Location = new-object System.Drawing.Size(140,250)
        $DFSBuildoutForm.Controls.Add($DFSRootSelectionLabel) 


        $DFSFolderPathLabel = New-Object System.Windows.Forms.Label
        $DFSFolderPathLabel.Text = "DFS Folder Target Path (Optional):"
        $DFSFolderPathLabel.AutoSize = $True
        $DFSFolderPathLabel.Location = new-object System.Drawing.Size(10,305)
        $DFSBuildoutForm.Controls.Add($DFSFolderPathLabel) 



        #################################  TEXT BOXES  #################################

        $DFSRootTextBox = new-object System.Windows.Forms.TextBox
        $DFSRootTextBox.Location = new-object System.Drawing.Size(10,175)
        $DFSRootTextBox.Size = new-object System.Drawing.Size(220,20)
            $DFSRootTextBox.Add_TextChanged({

                if($DFSRootTextBox.Text -match [regex]"^(?:[a-zA-Z]\:\\|\\\\)$") {
            
                [System.Windows.MessageBox]::Show("Error in DFS Root Text Box: Only input a DFS Root name, no file path necessary")
                $DFSRootTextBox.Clear()

                }

            })
        $DFSBuildoutForm.Controls.Add($DFSRootTextBox)


        $DFSFolderTextBox = new-object System.Windows.Forms.TextBox
        $DFSFolderTextBox.Location = new-object System.Drawing.Size(10,270)
        $DFSFolderTextBox.Size = new-object System.Drawing.Size(115,20)
            $DFSFolderTextBox.Add_TextChanged({

                if($DFSFolderTextBox.Text -match [regex]"^(?:[a-zA-Z]\:\\|\\\\)$") {
            
                [System.Windows.MessageBox]::Show("Error in DFS Folder Text Box: Only input a DFS Folder name, no file path necessary")
                $DFSFolderTextBox.Clear()

                }
                
                if($DFSFoldersListBox.SelectedItem -and $DFSFolderTextBox.Text.Length -ge 1) {

                $Global:DFSFolders[$DFSFoldersListBox.SelectedIndex] = $DFSFolderTextBox.Text

                }                            

            })
        $DFSBuildoutForm.Controls.Add($DFSFolderTextBox)


        $DFSTargetTextBox = new-object System.Windows.Forms.TextBox
        $DFSTargetTextBox.Location = new-object System.Drawing.Size(10,325)
        $DFSTargetTextBox.Size = new-object System.Drawing.Size(220,20)
            $DFSTargetTextBox.Add_TextChanged({

                if($DFSFoldersListBox.SelectedItem) {

                    if($DFSTargetTextBox.Text.Length -ge 1) {

                    $Global:DFSFolderTarget[$DFSFoldersListBox.SelectedIndex] = $DFSTargetTextBox.Text

                    }

                    else {
                    
                    $Global:DFSFolderTarget[$DFSFoldersListBox.SelectedIndex] = $Null
                    
                    }

                }

            })
        $DFSBuildoutForm.Controls.Add($DFSTargetTextBox)



        #################################  DROP DOWN BOXES  #################################

        $DropDownDFSRoot = new-object System.Windows.Forms.ComboBox
        $DropDownDFSRoot.Location = new-object System.Drawing.Size(140,270)
        $DropDownDFSRoot.Size = new-object System.Drawing.Size(90,20)
            $DropDownDFSRoot.Add_SelectedIndexChanged({

                if($DFSFoldersListBox.SelectedItem -and $DropDownDFSRoot.Text.Length -ge 1) {

                $Global:DFSFolderRoot[$DFSFoldersListBox.SelectedIndex] = $DropDownDFSRoot.Text

                }

            })
        $DFSBuildoutForm.Controls.Add($DropDownDFSRoot)



        #################################  BUTTONS  #################################

        $AddServerButton = new-object System.Windows.Forms.Button
        $AddServerButton.Location = new-object System.Drawing.Size(175,40)
        $AddServerButton.Size = new-object System.Drawing.Size(40,20)
        $AddServerButton.Text = ">>>"
            $AddServerButton.Add_Click({

                if($AllDFSServersListBox.SelectedItem){
                
                $SelectedServer = $AllDFSServersListBox.SelectedItem
                $DFSServersListBox.Items.Add($AllDFSServersListBox.SelectedItem)
                $AllDFSServersListBox.Items.Remove($AllDFSServersListBox.SelectedItem)
                $AllDFSServersListBox.ClearSelected()
                $DFSServersListBox.ClearSelected()
                
                $PrimaryDFSListedServers = @()
                $Counter = 0

                    while($Counter -lt $DFSServersListBox.Items.Count) {
                
                    $PrimaryDFSListedServers += $DFSServersListBox.Items[$Counter]

                    $Counter++

                    }

                    $DFSServersListBox.Items.Clear()

                    foreach($PrimaryDFSListedServer in ($PrimaryDFSListedServers | Sort)) {
                    
                    $DFSServersListBox.Items.Add($PrimaryDFSListedServer)

                    }
                
                EnableMakeDFSPrimary
                $Global:DFSServers.Insert($DFSServersListBox.Items.IndexOf(($DFSServersListBox.Items | where {$_ -match $SelectedServer})),($DFSServersListBox.Items | where {$_ -match $SelectedServer}))
                
                }

            })
        $DFSBuildoutForm.Controls.Add($AddServerButton) 


        $RemoveServerButton = new-object System.Windows.Forms.Button
        $RemoveServerButton.Location = new-object System.Drawing.Size(175,70)
        $RemoveServerButton.Size = new-object System.Drawing.Size(40,20)
        $RemoveServerButton.Text = "<<<"
            $RemoveServerButton.Add_Click({
            
                if($DFSServersListBox.SelectedItem){
                
                $Global:DFSServers.Remove($DFSServersListBox.SelectedItem)
                
                if($Global:DFSServers.Count -eq 1) {
                
                $Global:DFSServers[0] = "$($Global:DFSServers[0])*"
                
                }

                    if($DFSServersListBox.SelectedItem -match [regex]"\*") {

                    $AllDFSServersListBox.Items.Add(($DFSServersListBox.SelectedItem).Replace("*",""))
                    $DFSServersListBox.Items.Remove($DFSServersListBox.SelectedItem)

                    }

                    else {
                    
                    $AllDFSServersListBox.Items.Add($DFSServersListBox.SelectedItem)
                    $DFSServersListBox.Items.Remove($DFSServersListBox.SelectedItem)                 
                    
                    }

                $AllDFSServersListBox.ClearSelected()
                $DFSServersListBox.ClearSelected()

                $ListedServers = @()
                $Counter = 0
                
                    while($Counter -lt $AllDFSServersListBox.Items.Count) {
                
                    $ListedServers += $AllDFSServersListBox.Items[$Counter]

                    $Counter++
                    
                    }

                    $AllDFSServersListBox.Items.Clear()
                    
                    foreach($ListedServer in ($ListedServers | Sort)) {
                    
                    $AllDFSServersListBox.Items.Add($ListedServer)

                    }
                
                }

            EnableMakeDFSPrimary

            })
        $DFSBuildoutForm.Controls.Add($RemoveServerButton) 


        $MakeDFSPrimaryButton = new-object System.Windows.Forms.Button
        $MakeDFSPrimaryButton.Location = new-object System.Drawing.Size(165,100)
        $MakeDFSPrimaryButton.Size = new-object System.Drawing.Size(60,30)
        $MakeDFSPrimaryButton.Text = "  Make`nPrimary"
        $MakeDFSPrimaryButton.Enabled = $False
            $MakeDFSPrimaryButton.Add_Click({
            
                if($DFSServersListBox.SelectedItem){

                $OldDFSName = $DFSServersListBox.SelectedItem
                $Counter = 0

                    if($DFSServersListBox.Text -match [regex]"\*") {

                    $DFSServersListBox.Items.Remove($DFSServersListBox.Text)
                    $DFSServersListBox.Items.Add($OldDFSName.Replace("*",""))
                    $DFSServersListBox.ClearSelected()

                    }

                    else {

                       while($Counter -lt $DFSServersListBox.Items.Count) {
                        
                            if($DFSServersListBox.Items[$Counter] -match [regex]"\*"){

                            $NewPrimaryDFS = $DFSServersListBox.Items[$Counter].Replace("*","")
                            
                            $DFSServersListBox.Items.Remove($DFSServersListBox.Items[$Counter])
                            $DFSServersListBox.Items.Add($NewPrimaryDFS)
                               
                            }

                        $Counter++
                        
                        }
                    
                    $DFSServersListBox.Items.Remove($DFSServersListBox.SelectedItem)
                    $DFSServersListBox.Items.Add("$OldDFSName*")
                    $DFSServersListBox.ClearSelected()

                    }

                $PrimaryDFSListedServers = @()
                $Counter = 0

                    while($Counter -lt $DFSServersListBox.Items.Count) {
                
                    $PrimaryDFSListedServers += $DFSServersListBox.Items[$Counter]

                    $Counter++

                    }

                    $DFSServersListBox.Items.Clear()

                    foreach($PrimaryDFSListedServer in ($PrimaryDFSListedServers | Sort)) {
                    
                    $DFSServersListBox.Items.Add($PrimaryDFSListedServer)

                    }

                $OldPrimary = $Global:DFSServers | where {$_ -match [regex]"\*"}
                $Global:DFSServers[$Global:DFSServers.IndexOf($OldPrimary)] = $OldPrimary.Replace("*","")
                $Global:DFSServers[$Global:DFSServers.IndexOf($OldDFSName)] = "$($OldDFSName)*"
                
                }

            })
        $DFSBuildoutForm.Controls.Add($MakeDFSPrimaryButton) 


        $DFSBuildoutPreviousButton = new-object System.Windows.Forms.Button
        $DFSBuildoutPreviousButton.Location = new-object System.Drawing.Size(10,390)
        $DFSBuildoutPreviousButton.Size = new-object System.Drawing.Size(90,20)
        $DFSBuildoutPreviousButton.Text = "Previous $($Global:FinalForms.IndexOf("DFS")) / $($Global:FinalForms.Count)"
            $DFSBuildoutPreviousButton.Add_Click({
          
                if($SQLCheckbox.CheckState -eq "Checked") {
                    
                $DFSBuildoutForm.Hide()
                $SQLBuildoutForm.Show()
                     
                }

                elseif($CertificateServicesCheckbox.CheckState -eq "Checked") {
                    
                $DFSBuildoutForm.Hide() 
                $CertificateBuildoutForm.Show()
            
                }

                else {

                $DFSBuildoutForm.Hide() 
                $UserGroupOUForm.Show()
                    
                }

            })
        $DFSBuildoutForm.Controls.Add($DFSBuildoutPreviousButton) 


        $DFSBuildoutButton = new-object System.Windows.Forms.Button
        $DFSBuildoutButton.Location = new-object System.Drawing.Size(110,390)
        $DFSBuildoutButton.Size = new-object System.Drawing.Size(80,20)
        $DFSBuildoutButton.Text = "Finish"
        $DFSBuildoutButton.Enabled = $False
            $DFSBuildoutButton.Add_Click({
                
            $DFSBuildoutForm.Hide()
            StatusForm

            })
        $DFSBuildoutForm.Controls.Add($DFSBuildoutButton) 


        $DFSBuildoutValidateButton = new-object System.Windows.Forms.Button
        $DFSBuildoutValidateButton.Location = new-object System.Drawing.Size(200,390)
        $DFSBuildoutValidateButton.Size = new-object System.Drawing.Size(80,20)
        $DFSBuildoutValidateButton.Text = "Validate"
            $DFSBuildoutValidateButton.Add_Click({

            ValidateDFSCreation

            })
        $DFSBuildoutForm.Controls.Add($DFSBuildoutValidateButton) 


        $DFSBuildoutNextButton = new-object System.Windows.Forms.Button
        $DFSBuildoutNextButton.Location = new-object System.Drawing.Size(290,390)
        $DFSBuildoutNextButton.Size = new-object System.Drawing.Size(90,20)
        $DFSBuildoutNextButton.Text = "Exit"
            $DFSBuildoutNextButton.Add_Click({

            DisconnectXenHost
            $DFSBuildoutForm.Close()

            })
        $DFSBuildoutForm.Controls.Add($DFSBuildoutNextButton)


        $AddDFSRootButton = new-object System.Windows.Forms.Button
        $AddDFSRootButton.Location = new-object System.Drawing.Size(10,210)
        $AddDFSRootButton.Size = new-object System.Drawing.Size(100,20)
        $AddDFSRootButton.Text = "Add Root"
            $AddDFSRootButton.Add_Click({

                if($DFSRootTextBox.Text.Length -ge 1) {

                $DFSRootsListBox.Items.Add($DFSRootTextBox.Text)
                $DropDownDFSRoot.Items.Add($DFSRootTextBox.Text)
                
                    while($Global:DFSRoots.Count -ne $DFSRootsListBox.Items.Count) {
                    
                    $Global:DFSRoots += $Null
                    
                    }

                    foreach($Root in $DFSRootsListBox.Items | Sort) {
                
                    $DFSRootsListBox.Items.Remove($Root)
                    $DFSRootsListBox.Items.Add($Root)
                    
                    }

                $Global:DFSRoots.Insert($DFSRootsListBox.Items.IndexOf($DFSRootTextBox.Text),$DFSRootTextBox.Text)
                $DFSRootTextBox.Clear()

                }

            })
        $DFSBuildoutForm.Controls.Add($AddDFSRootButton) 


        $RemoveDFSRootButton = new-object System.Windows.Forms.Button
        $RemoveDFSRootButton.Location = new-object System.Drawing.Size(130,210)
        $RemoveDFSRootButton.Size = new-object System.Drawing.Size(100,20)
        $RemoveDFSRootButton.Text = "Remove Root"
        $RemoveDFSRootButton.Enabled = $False
            $RemoveDFSRootButton.Add_Click({

                if($DFSRootsListBox.SelectedItem) {

                $Global:DFSRoots.Remove($DFSRootsListBox.SelectedItem)
                $DropDownDFSRoot.Items.Remove($DFSRootsListBox.SelectedItem)
                $DFSRootsListBox.Items.Remove($DFSRootsListBox.SelectedItem)

                }

            })
        $DFSBuildoutForm.Controls.Add($RemoveDFSRootButton) 


        $AddDFSFolderButton = new-object System.Windows.Forms.Button
        $AddDFSFolderButton.Location = new-object System.Drawing.Size(10,355)
        $AddDFSFolderButton.Size = new-object System.Drawing.Size(100,20)
        $AddDFSFolderButton.Text = "Add Folder"
            $AddDFSFolderButton.Add_Click({

            AddDFSFolder

            })
        $DFSBuildoutForm.Controls.Add($AddDFSFolderButton) 


        $RemoveDFSFolderButton = new-object System.Windows.Forms.Button
        $RemoveDFSFolderButton.Location = new-object System.Drawing.Size(130,355)
        $RemoveDFSFolderButton.Size = new-object System.Drawing.Size(100,20)
        $RemoveDFSFolderButton.Text = "Remove Folder"
        $RemoveDFSFolderButton.Enabled = $False
            $RemoveDFSFolderButton.Add_Click({

                if($DFSFoldersListBox.SelectedItem) {

                $DFSFolderTextBox.Clear()
                $DFSTargetTextBox.Clear()
                $DropDownDFSRoot.ResetText()
                $Global:DFSFolders.RemoveAt($DFSFoldersListBox.SelectedIndex)
                $Global:DFSFolderRoot.RemoveAt($DFSFoldersListBox.SelectedIndex)
                $Global:DFSFolderTarget.RemoveAt($DFSFoldersListBox.SelectedIndex)
                $DFSFoldersListBox.Items.Remove($DFSFoldersListBox.SelectedItem)

                }

            })
        $DFSBuildoutForm.Controls.Add($RemoveDFSFolderButton) 


        $ClearDFSFolderButton = new-object System.Windows.Forms.Button
        $ClearDFSFolderButton.Location = new-object System.Drawing.Size(240,355)
        $ClearDFSFolderButton.Size = new-object System.Drawing.Size(140,20)
        $ClearDFSFolderButton.Text = "Clear Selected"
            $ClearDFSFolderButton.Add_Click({

                if($DFSFoldersListBox.SelectedItem) {

                $DFSFolderTextBox.Clear()
                $DropDownDFSRoot.ResetText()
                $DFSTargetTextBox.Clear()
                $DFSFoldersListBox.ClearSelected()
                
                }
            
            })
        $DFSBuildoutForm.Controls.Add($ClearDFSFolderButton)



        #################################  LIST BOXES  #################################

        $AllDFSServersListBox = new-object System.Windows.Forms.ListBox
        $AllDFSServersListBox.Location = new-object System.Drawing.Size(10,30)
        $AllDFSServersListBox.Size = new-object System.Drawing.Size(140,110)
        $DFSBuildoutForm.Controls.Add($AllDFSServersListBox)


        $DFSServersListBox = new-object System.Windows.Forms.ListBox
        $DFSServersListBox.Location = new-object System.Drawing.Size(240,30)
        $DFSServersListBox.Size = new-object System.Drawing.Size(140,110)
        $DFSBuildoutForm.Controls.Add($DFSServersListBox)


        $DFSRootsListBox = new-object System.Windows.Forms.ListBox
        $DFSRootsListBox.Location = new-object System.Drawing.Size(240,175)
        $DFSRootsListBox.Size = new-object System.Drawing.Size(140,60)
            $DFSRootsListBox.Add_SelectedIndexChanged({

                if($DFSRootsListBox.SelectedItem) {

                $RemoveDFSRootButton.Enabled = $True

                }

                else {
                
                $RemoveDFSRootButton.Enabled = $False
                
                }

            })
        $DFSBuildoutForm.Controls.Add($DFSRootsListBox)


        $DFSFoldersListBox = new-object System.Windows.Forms.ListBox
        $DFSFoldersListBox.Location = new-object System.Drawing.Size(240,270)
        $DFSFoldersListBox.Size = new-object System.Drawing.Size(140,90)
            $DFSFoldersListBox.Add_SelectedIndexChanged({

                if($DFSFoldersListBox.SelectedItem) {

                $DFSFolderTextBox.Text = $Global:DFSFolders[$DFSFoldersListBox.SelectedIndex]
                $DropDownDFSRoot.Text = $Global:DFSFolderRoot[$DFSFoldersListBox.SelectedIndex]
                $DFSTargetTextBox.Text = $Global:DFSFolderTarget[$DFSFoldersListBox.SelectedIndex]
                $RemoveDFSFolderButton.Enabled = $True

                }

                else {
                
                $RemoveDFSFolderButton.Enabled = $False
                
                }

            })
        $DFSBuildoutForm.Controls.Add($DFSFoldersListBox)
        

        foreach($DomainServer in ($Global:AllCreatedServers | sort)) {
        
        $AllDFSServersListBox.Items.Add($DomainServer)

        }


        #################################  Tool Tips  #################################
                	
        $FolderNameToolTip = New-Object System.Windows.Forms.ToolTip
        $FolderNameToolTip.SetToolTip($DFSFolderTextBox, "This folder will be created in the C:\DFSRoots\<NameSpace> folder")
        $FolderNameToolTip.AutoPopDelay = 10000
        $FolderNameToolTip.InitialDelay = 100
        $FolderNameToolTip.ToolTipTitle = "Folder Name Help"
        $FolderNameToolTip.ToolTipIcon = "Info"
        $FolderNameToolTip.IsBalloon = $True


        $TargetPathToolTip = New-Object System.Windows.Forms.ToolTip
        $TargetPathToolTip.SetToolTip($DFSTargetTextBox, "This Test Box will specify the path on the local machine that you want the folder, specified`nin the DFS Folder Text Box, to point to. For example, if you specify the Folder Name`nas 'Software' and the Folder Target as C:\Temp, the 'Software' folder still gets created in`nC:\DFSRoots\<NameSpace>, it will just redirect everything to C:\Temp")
        $TargetPathToolTip.AutoPopDelay = 20000
        $TargetPathToolTip.InitialDelay = 100
        $TargetPathToolTip.ToolTipTitle = "Folder Target Help"
        $TargetPathToolTip.ToolTipIcon = "Info"
        $TargetPathToolTip.IsBalloon = $True


    if($Global:DFSServers.Count -ge 1) {
        
        foreach($DFSServer in $Global:DFSServers) {

        $DFSServersListBox.Items.Add($DFSServer)

            if($DFSServer -match [regex]"\*") {
        
            $DFSServer = $DFSServer.Replace("*","")
        
            }

        $AllDFSServersListBox.Items.Remove($DFSServer)

        }

        if($DFSServersListBox.Items.Count -ge 2) {
        
        $MakeDFSPrimaryButton.Enabled = $True
        
        }
        
    }

    if($Global:DFSRoots.Count -ge 1) {
        
        foreach($DFSRoot in ($Global:DFSRoots | where {$_ -ne $Null})) {

        $DFSRootsListBox.Items.Add($DFSRoot)

        }
        
    }

    if($Global:DFSFolders.Count -ge 1) {
        
        foreach($DFSFolder in ($Global:DFSFolders | where {$_ -ne $Null})) {

        $DFSFoldersListBox.Items.Add($DFSFolder)

        }
        
    }


$DFSBuildoutForm.ShowDialog()

#################################  END FORM  #################################

}


Function ComponentInstallationForm {

#################################  COMPONENT INSTALLATION FORM  #################################

Add-Type -AssemblyName System.Windows.Forms
$ComponentInstallationForm = New-Object system.Windows.Forms.Form
$ComponentInstallationForm.Text = "AXL"
$ComponentInstallationForm.ClientSize = new-object System.Drawing.Size(190,160)
$ComponentInstallationForm.FormBorderStyle = 'Fixed3D'
$ComponentInstallationForm.MaximizeBox = $False
$ComponentInstallationForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $ComponentInstallationLabel = New-Object System.Windows.Forms.Label
        $ComponentInstallationLabel.Text = "Please Check the Components you Would Like to Configure:"
        $ComponentInstallationLabel.Size = New-Object System.Drawing.Size(165,30)
        $ComponentInstallationLabel.Location = new-object System.Drawing.Size(15,10)
        $ComponentInstallationForm.Controls.Add($ComponentInstallationLabel)



        #################################  CHECK BOXES  #################################

        $ActiveDirectoryCheckbox = New-Object System.Windows.Forms.Checkbox 
        $ActiveDirectoryCheckbox.Location = New-Object System.Drawing.Size(10,40) 
        $ActiveDirectoryCheckbox.Size = New-Object System.Drawing.Size(130,30) 
        $ActiveDirectoryCheckbox.Text = "Active Directory"
        $ActiveDirectoryCheckbox.Enabled = $False
        $ActiveDirectoryCheckbox.Checked = $True
        $ComponentInstallationForm.Controls.Add($ActiveDirectoryCheckbox)


        $CertificateServicesCheckbox = New-Object System.Windows.Forms.Checkbox 
        $CertificateServicesCheckbox.Location = New-Object System.Drawing.Size(10,70) 
        $CertificateServicesCheckbox.Size = New-Object System.Drawing.Size(130,30) 
        $CertificateServicesCheckbox.Text = "Certificate Services"
        $ComponentInstallationForm.Controls.Add($CertificateServicesCheckbox)

        $DFSCheckbox = New-Object System.Windows.Forms.Checkbox 
        $DFSCheckbox.Location = New-Object System.Drawing.Size(10,100)  
        $DFSCheckbox.Size = New-Object System.Drawing.Size(130,30) 
        $DFSCheckbox.Text = "File Services (DFS)"
        $ComponentInstallationForm.Controls.Add($DFSCheckbox)


        #################################  BUTTONS  #################################


        $ComponentInstallatioContinueButton = new-object System.Windows.Forms.Button
        $ComponentInstallatioContinueButton.Location = new-object System.Drawing.Size(10,130)
        $ComponentInstallatioContinueButton.Size = new-object System.Drawing.Size(80,20)
        $ComponentInstallatioContinueButton.Text = "Continue"
            $ComponentInstallatioContinueButton.Add_Click({
            
            $Global:VMCreationStartTime = Get-Date
            $ComponentInstallationForm.Hide()

                if($ActiveDirectoryCheckbox.CheckState -eq "Checked") { 
            
                $Global:FinalForms += "Domain" 
                $Global:FinalForms += "UGO"

                }

                if($CertificateServicesCheckbox.CheckState -eq "Checked") { 
            
                $Global:FinalForms += "Certificate" 
            
                }

                if($DFSCheckbox.CheckState -eq "Checked") { 
            
                $Global:FinalForms += "DFS" 
            
                }
            
            DomainBuildoutForm
            
            })
        $ComponentInstallationForm.Controls.Add($ComponentInstallatioContinueButton) 


        $ComponentInstallatioExitButton = new-object System.Windows.Forms.Button
        $ComponentInstallatioExitButton.Location = new-object System.Drawing.Size(100,130)
        $ComponentInstallatioExitButton.Size = new-object System.Drawing.Size(80,20)
        $ComponentInstallatioExitButton.Text = "Exit"
            $ComponentInstallatioExitButton.Add_Click({

            DisconnectXenHost
            $ComponentInstallationForm.Close()

            })
        $ComponentInstallationForm.Controls.Add($ComponentInstallatioExitButton) 


$ComponentInstallationForm.ShowDialog()

#################################  END FORM  #################################

}


#################################  PRESTAGE FORM  #################################

$PrestageForm = New-Object system.Windows.Forms.Form
$PrestageForm.Text = "AXL"
$PrestageForm.ClientSize = new-object System.Drawing.Size(200,90)
$PrestageForm.FormBorderStyle = 'Fixed3D'
$PrestageForm.MaximizeBox = $False
$PrestageForm.StartPosition = 'CenterScreen'


        #################################  LABELS  #################################

        $PrestageLabel = New-Object System.Windows.Forms.Label
        $PrestageLabel.Text = "What would you like to do?"
        $PrestageLabel.AutoSize = $True
        $PrestageLabel.Location = new-object System.Drawing.Size(35,10)
        $PrestageForm.Controls.Add($PrestageLabel)



        #################################  BUTTONS  #################################

        $PrestageYesButton = new-object System.Windows.Forms.Button
        $PrestageYesButton.Location = new-object System.Drawing.Size(35,30)
        $PrestageYesButton.ClientSize = new-object System.Drawing.Size(130,20)
        $PrestageYesButton.Text = "Create Custom ISO(s)"
            $PrestageYesButton.Add_Click({
            
            $PrestageForm.Hide()
            ISOConstructionForm            

            })
        $PrestageForm.Controls.Add($PrestageYesButton) 


        $PrestageNoButton = new-object System.Windows.Forms.Button
        $PrestageNoButton.Location = new-object System.Drawing.Size(35,60)
        $PrestageNoButton.ClientSize = new-object System.Drawing.Size(130,20)
        $PrestageNoButton.Text = "Create VMs"
            $PrestageNoButton.Add_Click({

            $PrestageForm.Hide()
            VMBuildoutForm

            })
        $PrestageForm.Controls.Add($PrestageNoButton) 


$PrestageForm.ShowDialog()

#################################  END FORM  #################################