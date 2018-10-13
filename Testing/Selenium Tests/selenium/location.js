const webdriver = require('selenium-webdriver');
const mysql = require('mysql');
const { expect } = require('chai');
const driver = new webdriver.Builder()
    .forBrowser('chrome')
    .setFirefoxOptions( /* … */)
    .setChromeOptions( /* … */)
    .build();

describe('Sanity' , function(){
    this.timeout(15000);
    before(function(done){
        driver.get('http://localhost:5000/').then(done);
    });

    after(function(done){
        driver.quit().then(done);
    });

    it('Login', async function(){
        await driver.findElement(webdriver.By.id("enterUserName")).sendKeys("paulhawit"); 
        await driver.findElement(webdriver.By.id("enterPassword")).sendKeys("jetson",webdriver.Key.ENTER); 
        var header = await driver.findElement(webdriver.By.id("locationSelectionText")).getText();
        expect(header).to.equal('To view information about a camera, please first select the location where the camera is located'); 
    });

    it('Check camera rooms in location', async function(){
        await driver.findElement(webdriver.By.id("1")).click();
        var roomsWeb =  await driver.findElement(webdriver.By.id("roomList")).getText();
        var connection = mysql.createConnection({
            host     : 'localhost',
            user     : 'root',
            password : 'password',
            database : 'jetson'
        });
        connection.connect(function(err) {
            if (err) {
              console.error('error connecting: ' + err.stack);
              return;
            }
            var sql = "SELECT * FROM jetson.camera WHERE user_id=1";
            connection.query(sql, function (err, result) {
                if (err) throw err;
                var roomData = [];
                for(var i = 0; i < result.length;i++){
                    roomData.push(result[i].monitored_area)
                }
                var resultString = roomData.join("");
                roomsWeb = roomsWeb.split("\n").join("");
                expect(resultString).to.equal(roomsWeb);
            });
            connection.end();
        })
    })

    it('Check camera info page', async function(){
        await driver.findElement(webdriver.By.id("1")).click();
        var titleWeb = await driver.findElement(webdriver.By.id("headerCamInfo")).getText();
        var connection = mysql.createConnection({
            host     : 'localhost',
            user     : 'root',
            password : 'password',
            database : 'jetson'
        });
        connection.connect(function(err) {
            if (err) {
              console.error('error connecting: ' + err.stack);
              return;
            }
            var sql = "SELECT * FROM jetson.camera WHERE user_id=1";
            connection.query(sql, function (err, result) {
                if (err) throw err;
                var titleData = result[0].camera_name;
                titleWeb = titleWeb.split("Here is the most recent information concerning the data captured by:\n").join("");
                expect(titleWeb).to.equal(titleData);
            });
            connection.end();
        })
    })

    
})