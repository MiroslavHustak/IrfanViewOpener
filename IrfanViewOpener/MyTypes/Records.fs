module Records

(*   
type InitialValuesRecord = 
    {
       low: int 
       high: int
       myMap: Map<string, int>
    }
*)

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


//************* for learning purposes only *********************

// moznost chyb
type Customer =
    { 
        CustomerId : string
        Email : string
        Telephone : string
        Address : string
    }

let createCustomer customerId email telephone address =
    { 
        //chybne prirazeni nezpusobi chybu, bo string vsade
        CustomerId = telephone
        Email = customerId
        Telephone = address
        Address = email 
    }

let customer =  createCustomer "C-123" "nicki@myemail.com" "029-293-23" "1 The Street" 

//***************************************************************************************
//spravne

type CustomerId1 = CustomerId1 of string
type Email1 = Email1 of string
type Telephone1 = Telephone1 of string
type Address1 = Address1 of string

type Customer1 =
    { 
        CustomerId1 : CustomerId1
        Email1 : Email1
        Telephone1 : Telephone1
        Address1 : Address1
    }

let createCustomer1 customerId email telephone address =
    { 
        //chybne prirazeni by vyhodilo chybu
        CustomerId1 = customerId
        Email1 = email
        Telephone1 = telephone
        Address1 = address 
    }

let customer1 =  createCustomer1
                 <|  CustomerId1 "C-123"
                 <|  Email1 "nicki@myemail.com"
                 <|  Telephone1 "029-293-23"
                 <|  Address1 "1 The Street"

//********************************************************

// A variant when only one contact detail is to be allowed at any point in time

type CustomerId2 = CustomerId2 of string   
   
type ContactDetails =
    | Address2 of string
    | Telephone2 of string
    | Email2 of string
              
type Customer2 =
    { 
        CustomerId2 : CustomerId2
        ContactDetails : ContactDetails            
    }

let createCustomer2 customerId oneContactDetail =
    { 
        //chybne prirazeni by vyhodilo chybu
        CustomerId2 = customerId
        ContactDetails = oneContactDetail
    }
   
let customer2 = createCustomer2
                <| CustomerId2 "Nicki"
                <| Email2 "nicki@myemail.com"

//****************************************************************************

 