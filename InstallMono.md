# Install Mono on RevolutionPi
Mono is a cross platform, open source .NET framework
For more information see http://www.mono-project.com/.

Use the packet installer.

    sudo apt-get install mono-complete

By default this installs Mono 3.x on current jessie releases.


If you want to use the lates supported release,
use the following stepr to upgrade to that (currently 5.0.1).

    sudo apt install apt-transport-https dirmngr
    sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
    echo "deb http://download.mono-project.com/repo/debian raspbianjessie main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
    sudo apt-get update
    sudo apt-get upgrade
    sudo apt-get install mono-complete

For current raspian stretch release change 'raspbianjessie' to 'raspbianstretch' in second command.
