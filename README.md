# Azure OpenAI MCP Agent Demo on top of Log Analyzer workspace

## Infrastructure
Obviously, that uses Azure infrastructure. You will need to create following resources (please, follow the sequence):
1. Azure OpenAI:
	1. Please, create chat completion deployment. `GPT-4o` is used in this case, but feel free to experiment with others.
2. Azure Log Analytics:
	1. Please, create Azure Log Analytics resource. Hint: after next steps, data will arrive into tables created under this resource. By default, data retention is set to 30 days, but you can easily modify it per table.
3. Azure Communication Services - Email:
	1. Please, create and setup email functionality with Azure Communication Services. [This link](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/create-email-communication-resource?pivots=platform-azp) can be a good start. Important: once setup, you need to have at least 3 following resources:
		1. Communication Service - your main Azure Communication Services resource
		2. Email Communication Service - ACS Email resource 
		3. Email Communication Services Domain - Resource holding link between ACS Email resource and domain to be used for email sending
	2. Go to `Diagnostic settings` and click on `Add diagnostic settings`. 
	Either check `allLogs` or select all email-related ones: `Email Service Send Mail Logs`, `Email Service Delivery Status Update Logs`, `Email Service User Engagement Logs`. 
	In `Destination details` click `Send to Log Analytics workspace` and select your freshly created `Log Analytics` resource.
	3. Verify using `Email -> Try Email` that messages can be sent. Got to `Monitoring -> Logs` and try some email-related queries. Please, keep in mind that logs might arrive with 5 minute delay.
4. Microsoft Entra ID and App Identity:
	1. Search for a following resource: `Microsoft Entra ID`.
	2. In this resource, go to `Manage -> App registrations` and click on `New registration`. Give it a name and leave all settings by default.
	3. Click on newly created registration, go to `Manage -> API permissions` and select `Add a permission`. Go to `APIs my organization uses` and search for `Log Analytics API`. Then `Delegated permissions`.
	Select `Data.Read` checkbox and then `Add permissions`. Validate that your newly created app has this permission as described here.
	4. Go to `Certificates & secrets` and click on `New client secret`. Give it description and save value of secret generated.

## Running locally
If you want to test it, you need two things:
1. Install docker
2. Create file, for example `.demo.env` and add the following content there:
```sh
AZURE_APP_CLIENT_ID=<Application (client) ID from application created in step 4; you may find it on resource Overview page>
AZURE_APP_CLIENT_SECRET=<Value of secret generated in step 4.4>
AZURE_APP_TENANT_ID=<Directory (tenant) ID from application created in step 4; you may find it on resource Overview page>
AZURE_LOG_ANALYTICS_WORKSPACE_ID=<Workspace ID of resource created in step 2; you may find it on resource Overview page>
AZURE_OPENAI_CHATGPT_DEPLOYMENT=<name of your chat gpt deployment>
AZURE_OPENAI_ENDPOINT=<endpoint of your Azure Open AI deployment>
AZURE_OPENAI_API_KEY=<key for using your Azure Open AI deployment>
AZURE_RESOURCE_ID=<resouce id created in step 1; you can copy it from the URL once you are in the resource, the format will be: /subscriptions/<subscription-id>/resourceGroups/<your-resource-group>/providers/Microsoft.Communication/CommunicationServices/<name-of-our-resource>
AZURE_CHAT_ENDPOINT=<Grab Endpoint from Settings -> Keys of your Communication Service created in step 3>
AZURE_CHAT_ACCESS_KEY=<Grab Key from Settings -> Keys of your Communication Service created in step 3>
```

Then you can do:
```sh
docker compose -f docker-compose.yml --env-file .demo.env build
docker compose -f docker-compose.yml --env-file .demo.env up -d
```

Application becomes available at `http://localhost`. Enjoy!

## Developing locally

### Backend
As code is written in `C#`, use `Visual Studio` (`Community Edition` is more than enough). 
Prior running it, you will need to setup the same variables mentioned for `Running locally` section, but as environment variables.
Then run it as `http`. It should open with `Swagger` on port `5247`.

### Frontend 

As this part is heavy on `TypeScript` (`React`, `Redux`, `Ant Design`), I recommend using `Visual Studio Code`. 
Depending on port used for `Backend` instance, you might want to adjust `BACKEND_HOST_URL` in `./build-configuration/dev.properties` file.
First, you need to do:
```sh
yarn
```
Then start it with:
```sh
yarn start
```
Application becomes available at `http://localhost:8080`.