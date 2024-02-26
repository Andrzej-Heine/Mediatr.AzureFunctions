
# Mediatr.AzureFunctions

.NET-based Azure Functions application that implement mediator pattern.


## Installation



- Microsoft Azure Storage Explorer https://azure.microsoft.com/en-us/products/storage/storage-explorer
- Postman https://www.postman.com/
- Install Visual Studio 2022 https://visualstudio.microsoft.com/pl/downloads/
- Install and start Azurite emulator https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage
    
## Running Azure Functions locally in Visual Studio 2022
- Open our Azure Function Visual Studio project
- Select the option Debug -> Start Debugging or simply press F5
- This action starts the Azure Functions runtime locally. If a Windows Defender Firewall appears, configure the necessary allow func to communicate options and click Allow access.
- We have created an Azure Function triggered by HTTP, our functions will be available at http://localhost:port/function-name
For example:
