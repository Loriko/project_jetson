var mysql = require('mysql');

var con = mysql.createConnection({
  host: "localhost",
  user: "root",
  password: "jetson",
  database: "mydb"
});

con.connect(function(err) {
  if (err) throw err;
  /*Select all customers with the address "Park Lane 38":*/
  con.query("SELECT * FROM mydb.perSecondStat WHERE numDetectedObjects = 32 AND ", function (err, result) {
    if (err) throw err;
    console.log(result)
  });
});
