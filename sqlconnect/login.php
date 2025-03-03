<?php

	$con = mysqli_connect('sql8.freesqldatabase.com', 'sql8686337', 'x8eRuNviVU', 'sql8686337');

	//check that connection happened
	if(mysqli_connect_errno())
	{
		echo "1: Connection Failed"; //error code #1 = connection failed
		exit();
	}

	$username = $_POST['name'];
	$password = $_POST["password"];

	//check if name already exists in database
	//run query
	$namecheckquery = "SELECT username, salt, hash, score FROM players WHERE username = '" . $username . "';";

	$namecheck = mysqli_query($con, $namecheckquery) or die("2: Name Check Query Failed"); // error code #2 - name check query failed

	if (mysqli_num_rows($namecheck) != 1)
	{
		echo "5: Either no user with name, or more than one"; //error code #5 - number of names matching != 1
		exit();
	}

	//get login info from query
	$existinginfo = mysqli_fetch_assoc($namecheck);
	$salt = $existinginfo["salt"];
	$hash = $existinginfo["hash"];

	$loginhash = crypt($password, $salt);
	if ($hash != $loginhash)
	{
		echo "6: Incorrect password"; //error code #6 - password does not has to match table
		exit();
	}

	//this is where you will place additional info
	echo "0\t" . $existinginfo["score"];
?>