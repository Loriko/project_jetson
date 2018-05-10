var webdriver = require('selenium-webdriver');
var mysql = require('mysql');

var client = new webdriver.Builder().withCapabilities({
  'browserName':'chrome'
}).build();
var chai = require('chai');
var expect = chai.expect;


describe('Administration Tests', function(){
    var url = 'localhost:5001';

    before(function(done){
        client.get(url).then(function(){
          done();
        })
    })

    after(function(done){
        client.quit().then(function(){
          done();
        })
    })

    it('Signed In', function(done){
        var usrNameBox = client.findElement(webdriver.By.name('username'));
        var usrName = "johndoe";
        var pwdBox = client.findElement(webdriver.By.name('password'));
        var pwd = "DMVIsAwesome123";

        usrNameBox.sendKeys(usrName);
        pwdBox.sendKeys(pwd);

        setTimeout(function () {
          client.findElement(webdriver.By.name('action')).click();
        }, 3000);

        setTimeout(function () {
          console.log("Page is loaded")
          client.findElement(webdriver.By.id('header')).getText().then(function(text){
              expect(text).to.equal('To view information about a camera, please first select the location where the camera is located');
              done();
          })
        }, 4000);
    })

    it('Navigate to data page', function(done){
      client.findElement(webdriver.By.id('DMV Walkley')).click();

      setTimeout(function () {
        client.findElement(webdriver.By.id('East Waiting Room')).click();
      }, 1000);

      setTimeout(function () {
        client.getCurrentUrl().then(function(value) {
            expect(value).to.equal('http://localhost:5001/Camera/CameraInformation?cameraId=1');
            done();
        });
      }, 3000);
    })

    it('Check value on webpage', function(done){
      done();
    })

})
