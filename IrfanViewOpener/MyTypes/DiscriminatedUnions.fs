namespace DiscriminatedUnions

type Result<'TSuccess,'TFailure> =

   | Success of 'TSuccess
   | Failure of 'TFailure

type TaskResults =     
    
   | TupleIntInt of inputValues: int*int
   | MapStringInt of myMap: Map<string, int>


