# profaninty filter

## setup

### if using VSCODE 

For running locally create a `.env` file in your working directory with the following content

```.env
CONNECTIONSTRINGS__FILTERS={CONNECTION STRING}
```

### Visual studio

either directly update `./sp-api-profanity-filter/appsettings.json` to have the requested connection string in the json

```json
  "ConnectionStrings": {
    "filters": "server={server};user={user};password={passsword};database={db}"
  } 
```

or update `./sp-api-profanity-filter/Properties/launchSettings.json` with the connection string.


## Debugging

You can use postman / curl / or some other service to make calls to the endpoint - some examples can be found here `./manual_tests/words.http`

```
curl -XPOST -H 'Authorization: __12345__' -H "Content-type: application/json" -d '{ "comment": "This is a crappy day" }' 'http://localhost:5000/api/profanity/filter/default/findDirty'
```