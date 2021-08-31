<?




if(strcasecmp($_SERVER['REQUEST_METHOD'], 'POST') != 0){
   //No post, don't run the script...
	exit();
}
 
//Make sure that the content type of the POST request has been set to application/json
$contentType = isset($_SERVER["CONTENT_TYPE"]) ? trim($_SERVER["CONTENT_TYPE"]) : '';
if(strcasecmp($contentType, 'application/json') != 0){
  //  throw new Exception('Content type must be: application/json');
	exit();
}
 
//Receive the RAW post data.
$content = trim(file_get_contents("php://input"));
 
//Attempt to decode the incoming RAW post data from JSON.
$decoded = json_decode($content, true);


//If json_decode failed, the JSON is invalid.
if(!is_array($decoded)){
	//do not pass go
   exit();
}else{

include("db.con.php"); //Your database connection strings etc.
include("login.functions.php");  //Your custom Logon Functions...

/*

At this point $decoded should be an array.  There should be two keys in the array, one is email the other is password. 

You absolutely must filter the incoming data before you insert it in to your database. 
Use filter_vars to filter and validate for email. 
For the password convert it to a salted MD5 hash or other hashing function of your choice. Never save plaintext passwords. 
You may want to also add in a 3rd variable, some type of hash that gets sent along from the client to prevent bots from spamming your url. 
If someone decides to decode your game or use wireshark to view net traffic they'll find the URL to your server pretty quick and could submit post requests to your server willy nilly with any data they choose. 

*/
$userinfo = NULL;

if(isset($decoded['email']) && isset($decoded['password']))){ //make sure these values exist before checking the password.



//Check email and password to log this user in and generate a session key.
$userinfo = checkPass($pdo,$decoded['email'],md5("saltedhash".$decoded['password'])));  

}

if(!is_array($userinfo)){

$info = array("pid"=>"","skey"=>"","instance"=>"","email"=>"","error"=>$userinfo);
echo json_encode($info);

}else{
echo json_encode($userinfo);
//print_r($decoded);  //Uncomment this to see the json data. 
}
//Logs a player in to the game..
}
?>
