module Json

open System.IO;
open Newtonsoft.Json
open System.Runtime.Serialization.Formatters.Binary

open Records
open Settings
open ROP_Functions.MyFunctions

//REDUNDANT CODE FOR LEARNING PURPOSES ONLY
//********************************************************************************************************************
// !!! without file.Exist testing and without try-with blocks !!!
   
let myRecord =   
    {
        low = Low 1000 
        high = High 2000
        myMap = MyMap (
                          Map.empty. 
                              Add("LT-01760",2).
                              Add("LT-01761",2).
                              Add("LT-01763",5).
                              Add("LT-01762",1).
                              Add("LT-01764",10)
                      )
    }

//*****Deserializace z Json (string) formatu
//In JSON, keys must be strings, written with double quotes. JSON vypada takto: {"low":1760,"high":1761,"myMap":{"LT-01760":2,"LT-01761":2,"LT-01762":1,"LT-01763":5,"LT-01764":10}}
let jsonText = """{"low":1760,"high":1761,"myMap":{"LT-01760":2,"LT-01761":2,"LT-01762":1,"LT-01763":5,"LT-01764":10}}""" //triple-quoted strings to allow using single quotes
let deSerializeFromJson = JsonConvert.DeserializeObject<InitialValuesRecord>(jsonText) 
   
//*****Serializace typu Record do Json a zpetna deserializace  
let serializeIntoJson = 
    JsonConvert.SerializeObject(myRecord)
    |> Option.ofObj
    |> optionToString1   

let deSerializeFromJson0 = JsonConvert.DeserializeObject<InitialValuesRecord>(serializeIntoJson)

//*****Serializace dalsiho typu Record do Json a zpetna deserializace 
let serializeIntoJson1 = 
    JsonConvert.SerializeObject(OpenIrfanView_Settings.Default) 
    |> Option.ofObj
    |> optionToString1   

let deSerializeFromJson1 = JsonConvert.DeserializeObject<OpenIrfanView_Settings>(serializeIntoJson1)

//*****Serializace Json formatu do binarniho souboru a zpetna deserializace    
let filepath = 
    Path.GetFullPath("json.dat")     
    |> Option.ofObj
    |> optionToString1   

let binaryFormatter: BinaryFormatter = new BinaryFormatter() 

//funguje to i s OpenIrfanView_Settings
let serialize = //musim do bloku, nebot use provede Close a Dispose po ukonceni daneho bloku
    let serializeIntoJsonAgain = serializeIntoJson
    let outputStream filepath = File.Create(filepath)
    use stream = outputStream filepath    
    do binaryFormatter.Serialize(stream, serializeIntoJsonAgain)  

let deserialize = 
    let inputStream filepath = File.Open(filepath, FileMode.Open, FileAccess.Read) 
    use stream = inputStream filepath
    let result: string = unbox (binaryFormatter.Deserialize(stream)) 
    result 

let deSerializeMyObjectAgain = JsonConvert.DeserializeObject<InitialValuesRecord>(deserialize)    
deSerializeMyObjectAgain |> ignore

