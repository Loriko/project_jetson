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

    it('Locations all visible', async function(){
        var locationWeb = await driver.findElement(webdriver.By.id("locations")).getText();
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
            var sql = "SELECT * FROM jetson.location";
            connection.query(sql, function (err, result) {
                if (err) throw err;
                var resultString = "";
                if(result.length <= 1){
                    resultString += result[0].location_name
                }
                else{
                    for(var i in result){
                        resultString += result[i].location_name+"\n"
                    }
                }
                expect(resultString).to.equal(locationWeb);
            });
            connection.end();
        })
    })

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

    it('Check Graph page', async function(){
        await driver.findElement(webdriver.By.id("camera-stats-link")).click();
        var graphHeaderWeb = await driver.findElement(webdriver.By.id("headerGraph")).getText();
        expect(graphHeaderWeb).to.equal("Statistics and Trends");
    })

    it('Users camera shown on manage camera page', async function(){
        await driver.findElement(webdriver.By.id("manageCamera")).click();
        await driver.findElement(webdriver.By.id("headerManageCamera"));
        var cameraWeb = await driver.findElement(webdriver.By.id("userCameras")).getText();
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
                var cameraData = [];
                for(var i = 0; i < result.length;i++){
                    cameraData.push(result[i].monitored_area)
                }
                var resultString = cameraData.join("");
                cameraWeb = cameraWeb.split("\nedit\n").join("");
                cameraWeb = cameraWeb.split("\nedit").join("");
                expect(resultString).to.equal(cameraWeb);
            });
            connection.end();
        })
    })

    it('Go to Alert page', async function(){
        await driver.findElement(webdriver.By.id("alerts")).click();
        var alertTitle = await driver.findElement(webdriver.By.id("alertTitle")).getText();
        expect(alertTitle).to.equal("Your Alerts");
    })

    it('Check alerts', async function(){
        var alertWebText = await driver.findElement(webdriver.By.id("alertList")).getText();

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
            var sql = "SELECT * FROM jetson.alert";
            connection.query(sql, function (err, result) {
                if (err) throw err;
                if(result.length < 1){
                    expect(alertWebText).to.equal("It seems you don't have any alert so far. Clicking the red floating button will allow you to set up an alert.")
                }
                else{
                    var resultString = alertWebText.split("\n");
                    expect(resultString.length).to.equal(result.length);
                }
            });
            connection.end();
        })
    })
    
    it('Check profile dropdown', async function(){
        await driver.findElement(webdriver.By.id('profile')).click();
        await driver.sleep(2000);
        var signOutText = await driver.findElement(webdriver.By.id("signOut")).getText();
        var modPerText = await driver.findElement(webdriver.By.id("modifyPersonalSettings")).getText();
        expect(signOutText).to.equal("Sign Out");
        expect(modPerText).to.equal("Modify Personal Settings");
    })

    it('Sign out', async function(){
        await driver.findElement(webdriver.By.id("signOut")).click();
        var title = await driver.findElement(webdriver.By.id("headerHome")).getText();
        expect(title).to.equal("Welcome to the Real Time Object Monitoring System");
    })
});