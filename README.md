# Baby NI Backend 

This project is a microservice project where the main project "NI-BE" communicates with the other APIS: babyNI-LoaderAPI, babyNI-AggregatorAPI.
Each API is expected to receive a file containing the data required. Which means
each API can be deployed on different server.


## NI-BE
This applicaiton is where the worker(watcher) is listening to a specific folder.
Once a file is dropped it will directly calls the parser to parser the file based on certain criteria.

```bash
### Setup the environment variables for NI-BE
"HOST":Database IP ,
"DATABASE":Database name,
"USERNAME":Database username ,
"PASSWORD": Database password,
"table_Radio_link_power": Table name of Radio Link Power,
"table_RF_Input_power": Table name of RF Input Power,
"watcherFolder": Watcher folder locatiom,
"parserFolder":  Parser folder location,
"loadedData": Loaded data folder location,
"oldDataFolder": old data folder location,
"newDataFolder": new data folder location,
"rejectedPath": rejectedPath location,
"exceptionPath": exceptionPath location,
"appUrl": The application url,
"loaderUrl": The loader url 
```
```bash
###API
 POST

| EndPoint                 | Body     | Description                |
| :----------------------- | :------- | :------------------------- |
| `api/Parser/parser-file` | `file`   | **Required**.              |

```



## babyNI-LoaderAPI
After successfully parsing the file, the Loader API will be called and load the data to the database based on certain criteria.

```bash
### Setup the environment variables for babyNI-LoaderAPI
"HOST":Database IP ,
"DATABASE":Database name,
"USERNAME":Database username ,
"PASSWORD": Database password,
"table_Radio_link_power": Table name of Radio Link Power,
"table_RF_Input_power": Table name of RF Input Power,
"parserFolder":  Parser folder location,
"loadedData": Loaded data folder location,
"rejectedPath": rejectedPath location,
"exceptionPath": exceptionPath location,
"appUrl": The application url,
"aggregationUrl": The aggregator url 
```

```bash
###API
 POST

| EndPoint               | Body     | Description                |
| :--------------------- | :------- | :------------------------- |
| `api/Loader/load-file` | `file`   | **Required**.              |
```

## babyNI-AggregatorAPI
Once the data is loaded, the Aggregator API will be called and create the requested KPIs.

```bash
### Setup the environment variables for babyNI-AggregatorAPI
"HOST":Database IP ,
"DATABASE":Database name,
"USERNAME":Database username ,
"PASSWORD": Database password,
"table_Radio_link_power": Table name of Radio Link Power,
"table_RF_Input_power": Table name of RF Input Power,
"parserFolder":  Parser folder location,
"loadedData": Loaded data folder location,
"rejectedPath": rejectedPath location,
"exceptionPath": exceptionPath location,
"appUrl": The application url,
```
```bash
###API
 POST

| EndPoint                          | Body     | Description                |
| :-------------------------------- | :------- | :------------------------- |
| `api/Aggregation/aggreagate-file` | `file`   | **Required**.              |
```

## babyNI-GetDataAPI
This API is responsible for fetching the data from the database based on what the UI is requesting.

```bash
### Setup the environment variables for babyNI-GetDataAPI
"HOST":Database IP ,
"DATABASE":Database name,
"USERNAME":Database username ,
"PASSWORD": Database password,
```
```bash
###API
 POST

| EndPoint                 | Body          | Description                |
| :----------------------- | :------------ | :------------------------- |
| `api/GetData/get-data`   | `dataModel`   | **Required**.              |

dataModel:{
  "neRequested": "string",
  "time_stamp": "string",
  "aggTime": "string"
}
```
