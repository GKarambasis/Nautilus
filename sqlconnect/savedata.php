<?php

	$con = mysqli_connect('localhost', 'id21916966_dampdatabase', 'xProject123Damp!', 'id21916966_projectdamp');

	//check that connection happened
	if(mysqli_connect_errno())
	{
		echo "1: Connection Failed"; //error code #1 = connection failed
		exit();
	}

	$username = $_POST["name"];
	$newupgrade1 = $_POST["upgrade1"];
	$newupgrade2 = $_POST["upgrade2"];
	$newupgrade3 = $_POST["upgrade3"];
	$newscrap = $_POST["scrap"]

	$namecheckquery = "SELECT username FROM players WHERE username = '" . $username . "';";

	$namecheck = mysqli_query($con, $namecheckquery) or die("2: Name Check Query Failed");

	if (mysqli_num_rows($namecheck) != 1)
	{
		echo "5: Either no user with name, or more than one"; //error code #5 - number of names matching != 1
		exit();
	}



	$updatequery1 = "UPDATE players SET upgrade1 = " . $newupgrade1 . " WHERE username = '" . $username . "';";

	$updatequery2 = "UPDATE players SET upgrade2 = " . $newupgrade2 . " WHERE username = '" . $username . "';";

	$updatequery3 = "UPDATE players SET upgrade3 = " . $newupgrade3 . " WHERE username = '" . $username . "';";

	$updatequery4 = "UPDATE players SET scrap = " . $newscrap . " WHERE username = '" . $username . "';";

	mysqli_query($con, $updatequery1) or die("7: Save Query Failed")//error code #7 - Update Query Failed
	mysqli_query($con, $updatequery2) or die("7: Save Query Failed")//error code #7 - Update Query Failed
	mysqli_query($con, $updatequery3) or die("7: Save Query Failed")//error code #7 - Update Query Failed
	mysqli_query($con, $updatequery4) or die("7: Save Query Failed")//error code #7 - Update Query Failed

	echo "0";
?>