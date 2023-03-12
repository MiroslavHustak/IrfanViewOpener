module Records

//viz Isaac Abraham str 272 a dale
//vyzkouseni si wrappers with SDCUs pro zabraneni mixing types

type Low = Low of int
type High = High of int
type MyMap = MyMap of Map<string, int>

type InitialValuesRecord =
    { 
        low: Low 
        high: High
        myMap: MyMap
    }

(*   
type InitialValuesRecord = 
    {
       low: int 
       high: int
       myMap: Map<string, int>
    }
*) 