-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `mydb` DEFAULT CHARACTER SET utf8 ;
USE `mydb` ;

-- -----------------------------------------------------
-- Table `mydb`.`User`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`User` (
  `idUser` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(45) NOT NULL,
  `password` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idUser`),
  UNIQUE INDEX `username_UNIQUE` (`username` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Address`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Address` (
  `idAddress` INT NOT NULL AUTO_INCREMENT,
  `location` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idAddress`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Camera`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Camera` (
  `idCamera` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `User_idUser` INT UNSIGNED NOT NULL,
  `cameraName` VARCHAR(45) NULL,
  `Address_idAddress` INT NOT NULL,
  PRIMARY KEY (`idCamera`, `User_idUser`, `Address_idAddress`),
  INDEX `fk_Camera_User_idx` (`User_idUser` ASC),
  INDEX `fk_Camera_Address1_idx` (`Address_idAddress` ASC),
  CONSTRAINT `fk_Camera_User`
    FOREIGN KEY (`User_idUser`)
    REFERENCES `mydb`.`User` (`idUser`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Camera_Address1`
    FOREIGN KEY (`Address_idAddress`)
    REFERENCES `mydb`.`Address` (`idAddress`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`perSecondStat`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`perSecondStat` (
  `idStat` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Camera_idCamera` INT UNSIGNED NOT NULL,
  `Camera_User_idUser` INT UNSIGNED NOT NULL,
  `date` DATE NOT NULL,
  `time` TIME NOT NULL,
  `numObject` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`idStat`, `Camera_idCamera`, `Camera_User_idUser`),
  UNIQUE INDEX `idStat_UNIQUE` (`idStat` ASC),
  CONSTRAINT `fk_perSecondStat_Camera1`
    FOREIGN KEY (`Camera_idCamera` , `Camera_User_idUser`)
    REFERENCES `mydb`.`Camera` (`idCamera` , `User_idUser`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`perHourStat`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`perHourStat` (
  `idHourStat` INT NOT NULL,
  PRIMARY KEY (`idHourStat`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`perSecondStat_has_perHourStat`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`perSecondStat_has_perHourStat` (
  `perSecondStat_idStat` INT UNSIGNED NOT NULL,
  `perSecondStat_Camera_idCamera` INT UNSIGNED NOT NULL,
  `perSecondStat_Camera_User_idUser` INT UNSIGNED NOT NULL,
  `perHourStat_idHourStat` INT NOT NULL,
  PRIMARY KEY (`perSecondStat_idStat`, `perSecondStat_Camera_idCamera`, `perSecondStat_Camera_User_idUser`, `perHourStat_idHourStat`),
  INDEX `fk_perSecondStat_has_perHourStat_perHourStat1_idx` (`perHourStat_idHourStat` ASC),
  INDEX `fk_perSecondStat_has_perHourStat_perSecondStat1_idx` (`perSecondStat_idStat` ASC, `perSecondStat_Camera_idCamera` ASC, `perSecondStat_Camera_User_idUser` ASC),
  CONSTRAINT `fk_perSecondStat_has_perHourStat_perSecondStat1`
    FOREIGN KEY (`perSecondStat_idStat` , `perSecondStat_Camera_idCamera` , `perSecondStat_Camera_User_idUser`)
    REFERENCES `mydb`.`perSecondStat` (`idStat` , `Camera_idCamera` , `Camera_User_idUser`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_perSecondStat_has_perHourStat_perHourStat1`
    FOREIGN KEY (`perHourStat_idHourStat`)
    REFERENCES `mydb`.`perHourStat` (`idHourStat`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
