Citrix XenServer Tools
======================

This CD contains the Citrix XenServer Tools and Drivers for supported
guest operating systems. You will need to install them to get the best
performance from your virtual machine, and to access advanced features
such as XenMotion.

Windows
-------

Windows users should install the Windows drivers using the setup.exe 
installer application in the top-level of this CD. This setup 
utility will install the latest versions of the drivers, and 
automatically upgrade any older versions.

Linux
-----

Linux users need to install the guest tools from the /Linux directory on
this CD. This will ensure your Linux VM has access to advanced features
such as XenMotion and in-guest performance metrics.

In addition, we have provided a number of kernel files, which are mostly
based on the vendor-provided kernels, but provide specific enhancements
for improved stability and performance when running on XenServer.

You can install the required packages by running install.sh like so:

$ <mnt>/Linux/install.sh

where <mnt> is the CD mount point.

To omit the kernel upgrade pass the -k flag to install.sh.

