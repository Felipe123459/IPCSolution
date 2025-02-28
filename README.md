# IPCSolution
This part of the project was about implementing an Inter-Process communiction program using linux or self made pipelines, in order to communicate with eachother by sending data. The entirety of this project was written is C# for its threads application, i decided to build a simple pipeline using my own programs to process data.
Instructions for installing and execution-
1.Open powershell as an administrator
2.run wsl --install
3.run wsl --list --verbose. if ubuntu is installed it will be displayed
4.if WSl is installed it needs to be updated, run wsl --update
5.run wsl --set-default-version 2
6.run wsl --install -d Ubuntu to install ubuntu
7.update package list, run sudo apt update && sudo apt upgrade -y
8.install dpendencies, run sudo apt install -y wget gpg
9.Add the microsoft key, run wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor | sudo tee /usr/share/keyrings/packages.microsoft.gpg > /dev/null echo "deb [signed-by=/usr/share/keyrings/packages.microsoft.gpg] https://packages.microsoft.com/repos/code stable main" | sudo tee /etc/apt/sources.list.d/vscode.list
10.install VSCode, run sudo apt update
11.run sudo apt install -y code
12.verify version, run code --version
13.install dependencies for SDK, run sudo apt update
14.run sudo apt install -y wget apt-transport-https software-properties-common
15.run wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb sudo dpkg -i packages-microsoft-prod.deb
16.install SDK, run sudo apt install -y dotnet-sdk-8.0
17.verify version, run dotnet --version
18.run cd IPC
19.toif you wish to run the program with the three componens then run, dotnet run -- run-pipeline
20. if you want to run the components individually then dotnet run -- generator, however for the transformer and comsumer you will need to insert data, eg  echo -e "apple,5,red" | dotnet run --transformer, or  echo -e "apple,5,red" | dotnet run --consumer
