-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `mydb` ;

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `jetson` DEFAULT CHARACTER SET utf8 ;
USE `jetson` ;

-- -----------------------------------------------------
-- Table `mydb`.`location`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`location` ;

CREATE TABLE IF NOT EXISTS `jetson`.`location` (
  `id` INT(11) NOT NULL,
  `location_name` VARCHAR(45) NOT NULL,
  `address_line` VARCHAR(200) NULL,
  `city` VARCHAR(45) NULL,
  `state` VARCHAR(45) NULL,
  `zip` VARCHAR(45) NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`camera`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`camera` ;

CREATE TABLE IF NOT EXISTS `jetson`.`camera` (
  `id` INT(10) UNSIGNED NOT NULL,
  `camera_name` VARCHAR(45) NULL DEFAULT NULL,
  `location_id` INT(11) NOT NULL,
  PRIMARY KEY (`id`, `location_id`),
  INDEX `fk_camera_address_idx` (`location_id` ASC),
  CONSTRAINT `fk_camera_Address`
    FOREIGN KEY (`location_id`)
    REFERENCES `jetson`.`location` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`perhourstat`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`perhourstat` ;

CREATE TABLE IF NOT EXISTS `jetson`.`perhourstat` (
  `id` INT(11) NOT NULL,
  `num_detected_object` INT NULL,
  `max_object` INT NULL,
  `min_object` INT NULL,
  `avg_object` INT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`persecondstat`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`persecondstat` ;

CREATE TABLE IF NOT EXISTS `jetson`.`persecondstat` (
  `id` INT(10) UNSIGNED NOT NULL,
  `camera_id` INT(10) UNSIGNED NOT NULL,
  `num_detected_object` INT(11) NOT NULL DEFAULT '0',
  `date_time` DATETIME NOT NULL,
  `has_saved_image` TINYINT NULL DEFAULT 0,
  PRIMARY KEY (`id`, `camera_id`),
  UNIQUE INDEX `idStat_UNIQUE` (`id` ASC),
  INDEX `fk_persecondstat_camera` (`camera_id` ASC),
  CONSTRAINT `fk_persecondstat_camera`
    FOREIGN KEY (`camera_id`)
    REFERENCES `jetson`.`camera` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`persecondstat_has_perhourstat`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`persecondstat_has_perhourstat` ;

CREATE TABLE IF NOT EXISTS `jetson`.`persecondstat_has_perhourstat` (
  `per_second_stat_id` INT(10) UNSIGNED NOT NULL,
  `per_hour_stat_id` INT(11) NOT NULL,
  PRIMARY KEY (`per_second_stat_id`, `per_hour_stat_id`),
  INDEX `fk_persecondstat_has_perhourstat_perhourstat_idx` (`per_hour_stat_id` ASC),
  INDEX `fk_persecondstat_has_perhourstat_persecondstat_idx` (`per_second_stat_id` ASC),
  CONSTRAINT `fk_persecondstat_has_perhourstat_perhourstat`
    FOREIGN KEY (`per_hour_stat_id`)
    REFERENCES `jetson`.`perhourstat` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_persecondstat_has_perhourstat_persecondstat`
    FOREIGN KEY (`per_second_stat_id`)
    REFERENCES `jetson`.`persecondstat` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`user`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`user` ;

CREATE TABLE IF NOT EXISTS `jetson`.`user` (
  `id` INT(10) UNSIGNED NOT NULL,
  `username` VARCHAR(45) NOT NULL,
  `password` VARCHAR(45) NOT NULL,
  `api_key` VARCHAR(45) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `username_UNIQUE` (`username` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `mydb`.`user_came_association`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `mydb`.`user_came_association` ;

CREATE TABLE IF NOT EXISTS `jetson`.`user_came_association` (
  `camera_id` INT(10) UNSIGNED NOT NULL,
  `user_id` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`camera_id`, `user_id`),
  INDEX `fk_camera_has_user_user_idx` (`user_id` ASC),
  INDEX `fk_camera_has_user_camera_idx` (`camera_id` ASC),
  CONSTRAINT `fk_camera_has_user_camera`
    FOREIGN KEY (`camera_id`)
    REFERENCES `jetson`.`camera` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_camera_has_user_user`
    FOREIGN KEY (`user_id`)
    REFERENCES `jetson`.`user` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
