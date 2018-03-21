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
-- Table `mydb`.`address`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`address` (
  `id` INT(11) NOT NULL,
  `locationName` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`camera`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`camera` (
  `id` INT(10) UNSIGNED NOT NULL,
  `cameraName` VARCHAR(45) NULL DEFAULT NULL,
  `Address_id` INT(11) NOT NULL,
  PRIMARY KEY (`id`, `Address_id`),
  INDEX `fk_Camera_Address1_idx` (`Address_id` ASC),
  CONSTRAINT `fk_Camera_Address1`
    FOREIGN KEY (`Address_id`)
    REFERENCES `mydb`.`address` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`perhourstat`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`perhourstat` (
  `id` INT(11) NOT NULL,
  `perhourstat` VARCHAR(45) NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`persecondstat`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`persecondstat` (
  `id` INT(10) UNSIGNED NOT NULL,
  `Camera_id` INT(10) UNSIGNED NOT NULL,
  `numDetectedObject` INT(11) NOT NULL DEFAULT '0',
  `dateTime` DATETIME NOT NULL,
  `hasSavedImage` TINYINT NULL DEFAULT 0,
  PRIMARY KEY (`id`, `Camera_id`),
  UNIQUE INDEX `idStat_UNIQUE` (`id` ASC),
  INDEX `fk_perSecondStat_Camera1` (`Camera_id` ASC),
  CONSTRAINT `fk_perSecondStat_Camera1`
    FOREIGN KEY (`Camera_id`)
    REFERENCES `mydb`.`camera` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`persecondstat_has_perhourstat`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`persecondstat_has_perhourstat` (
  `perSecondStat_id` INT(10) UNSIGNED NOT NULL,
  `perSecondStat_Camera_id` INT(10) UNSIGNED NOT NULL,
  `perHourStat_id` INT(11) NOT NULL,
  PRIMARY KEY (`perSecondStat_id`, `perSecondStat_Camera_id`, `perHourStat_id`),
  INDEX `fk_perSecondStat_has_perHourStat_perHourStat1_idx` (`perHourStat_id` ASC),
  INDEX `fk_perSecondStat_has_perHourStat_perSecondStat1_idx` (`perSecondStat_id` ASC, `perSecondStat_Camera_id` ASC),
  CONSTRAINT `fk_perSecondStat_has_perHourStat_perHourStat1`
    FOREIGN KEY (`perHourStat_id`)
    REFERENCES `mydb`.`perhourstat` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_perSecondStat_has_perHourStat_perSecondStat1`
    FOREIGN KEY (`perSecondStat_id` , `perSecondStat_Camera_id`)
    REFERENCES `mydb`.`persecondstat` (`id` , `Camera_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`user`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`user` (
  `id` INT(10) UNSIGNED NOT NULL,
  `username` VARCHAR(45) NOT NULL,
  `password` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `username_UNIQUE` (`username` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`user_came_association`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`user_came_association` (
  `camera_id` INT(10) UNSIGNED NOT NULL,
  `user_id` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`camera_id`, `user_id`),
  INDEX `fk_camera_has_user_user1_idx` (`user_id` ASC),
  INDEX `fk_camera_has_user_camera1_idx` (`camera_id` ASC),
  CONSTRAINT `fk_camera_has_user_camera1`
    FOREIGN KEY (`camera_id`)
    REFERENCES `mydb`.`camera` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_camera_has_user_user1`
    FOREIGN KEY (`user_id`)
    REFERENCES `mydb`.`user` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
