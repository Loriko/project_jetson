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
})