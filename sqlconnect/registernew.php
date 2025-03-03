<?php

	$con = mysqli_connect('localhost', 'id21916966_dampdatabase', 'xProject123Damp!', 'id21916966_projectdamp');
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
	$namecheckquery = "SELECT username FROM players WHERE username = '" . $username . "';";

	$namecheck = mysqli_query($con, $namecheckquery) or die("2: Name Check Query Failed"); // error code #2 - name check query failed

	if (mysqli_num_rows($namecheck) > 0)
	{
		echo "3: Name already exists"; //error code #3 - name exists, cannot register
		exit();
	}

	//add the user to the table 
	//create players password as salty hash
	$salt = "\$5\$rounds=5000\$" . "steamedhams" . $username . "\$";
	$hash = crypt($password, $salt);
	$insertuserquery = "INSERT INTO players (username, hash, salt) VALUES ('" . $username . "', '" . $hash . "', '" . $salt . "');";
	mysqli_query($con, $insertuserquery) or die("4: Insert Player Query Failed"); // error code #4 - insert query failed

	echo("0");


?>