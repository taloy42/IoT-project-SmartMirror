
# VerifyFace.cs

## GetPersonDetails

### input
* query:
    * @personGroupId
    * @personId

### output
* jsonString - the json {name:p.Name, userData:p.UserData} as a string

## TryToMatch

### input
* query:
    * @personGroupId [@imgBase64Enc]
* body(files):
    * @image

### output
* Guid - finding the best match in @personGroupId for @image (if @imgBase64Enc is not provided)

## TryToMatchDetails

### input
* query:
    * @personGroupId [@imgBase64Enc]
* body(files):
    * @image

### output
* jsonString - the json 
{
    personId:Guid,
    persistedFaceIdsuserData:List[GUID],
    name:String,
    userData:
        {
            firstName:String,
            lastName:String
        }
}
 as a string, of the best match in @personGroupId for @image (if @imgBase64Enc is not provided)

## CreatePerson

### input
* query/body:	
    * @username
    * @personGroupId
    * @jsonData - data about the person packed in a string representing a json
			
### output
* @person - the person that was created, of type persons

## AddFaceToPerson

### input
* query:
    * @guid - guid of specific person
    * @personGroupId
    * [@imgBase64Enc]
* body[files]:
    * @image

### output
* no output - adds @image to the person represented by @guid in @personGroupId (if @imgBase64Enc is not given)



# CreateConnection.cs

## negotitate

### input
* no input
### output
* the connectionInfo to signalR


# GetCounter.cs

## GetCounter

### input
* no input
### output
* output current @coutner

# HttpExample.cs

## HttpExample - func for tests

### input
* changes based on tests
### output
* changes based on tests

## ExampleURL

### input
* no input
### output
* no output - dummy function

# MessageReciever.cs

## MessageReceiver

### input
* no input
### output
* no output - incrementing @counter if a message arrived, and relaying to every connected device

# UpdateCounter.cs

## UpdateCounter

### input
* query/body
    * @counter
### output
* message with the new counter value
