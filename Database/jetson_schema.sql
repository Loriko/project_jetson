-- -----------------------------------------------------
-- Schema jetson
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `jetson` ;

CREATE SCHEMA IF NOT EXISTS `jetson` DEFAULT CHARACTER SET utf8 ;
USE `jetson` ;

-- -----------------------------------------------------
-- Table `jetson`.`location`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`location` ;

CREATE TABLE IF NOT EXISTS `jetson`.`location` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `location_name` VARCHAR(45) NOT NULL,
  `address_line` VARCHAR(200) NULL,
  `city` VARCHAR(45) NULL,
  `state` VARCHAR(45) NULL,
  `zip` VARCHAR(45) NULL,
  PRIMARY KEY (`id`))
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`camera`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`camera` ;

CREATE TABLE IF NOT EXISTS `jetson`.`camera` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `camera_name` VARCHAR(45) NULL DEFAULT NULL,
  `location_id` INT(11) NOT NULL,
  `user_id` INT(11) NOT NULL,
  `monitored_area` VARCHAR(45) NOT NULL,
  `brand` VARCHAR(45) NULL,
  `model` VARCHAR(45) NULL,
  `resolution` VARCHAR(45) UNSIGNED NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_camera_address_idx` (`location_id` ASC),
  CONSTRAINT `fk_camera_Address`
    FOREIGN KEY (`location_id`)
    REFERENCES `jetson`.`location` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`per_hour_stat`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`per_hour_stat` ;

CREATE TABLE IF NOT EXISTS `jetson`.`per_hour_stat` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `date_day` DATE NOT NULL,
  `date_hour` INT(2) UNSIGNED NOT NULL,
  `max_detected_object` INT UNSIGNED NULL,
  `min_detected_object` INT UNSIGNED NULL,
  `avg_detected_object` FLOAT UNSIGNED NULL,
  PRIMARY KEY (`id`))
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`per_second_stat`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`per_second_stat` ;

CREATE TABLE IF NOT EXISTS `jetson`.`per_second_stat` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `camera_id` INT(10) UNSIGNED NOT NULL,
  `num_detected_object` INT(11) NOT NULL DEFAULT 0,
  `date_time` DATETIME NOT NULL,
  `has_saved_image` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `idStat_UNIQUE` (`id` ASC),
  INDEX `fk_per_second_stat_camera` (`camera_id` ASC),
  CONSTRAINT `fk_per_second_stat_camera`
    FOREIGN KEY (`camera_id`)
    REFERENCES `jetson`.`camera` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`per_second_stat_per_hour_stat_association`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`per_second_stat_per_hour_stat_association` ;

CREATE TABLE IF NOT EXISTS `jetson`.`per_second_stat_per_hour_stat_association` (
  `per_second_stat_id` INT(10) UNSIGNED NOT NULL,
  `per_hour_stat_id` INT(11) NOT NULL,
  PRIMARY KEY (`per_second_stat_id`, `per_hour_stat_id`),
  INDEX `fk_per_second_stat_per_hour_stat_association_per_hour_stat_idx` (`per_hour_stat_id` ASC),
  INDEX `fk_per_second_stat_per_hour_stat_association_per_second_stat_idx` (`per_second_stat_id` ASC),
  CONSTRAINT `fk_per_second_stat_per_hour_stat_association_per_hour_stat`
    FOREIGN KEY (`per_hour_stat_id`)
    REFERENCES `jetson`.`per_hour_stat` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_per_second_stat_per_hour_stat_association_per_second_stat`
    FOREIGN KEY (`per_second_stat_id`)
    REFERENCES `jetson`.`per_second_stat` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`user`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`user` ;

CREATE TABLE IF NOT EXISTS `jetson`.`user` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(45) NOT NULL,
  `password` VARCHAR(45) NOT NULL,
  `api_key` VARCHAR(45) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `username_UNIQUE` (`username` ASC))
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`user_camera_association`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`user_camera_association` ;

CREATE TABLE IF NOT EXISTS `jetson`.`user_camera_association` (
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
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`api_key`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`api_key` ;

CREATE TABLE IF NOT EXISTS `jetson`.`api_key` (
  `id` INT(8) UNSIGNED NOT NULL AUTO_INCREMENT,
  `key` CHAR(32) NOT NULL,
  `salt` VARCHAR(24) NOT NULL,
  `is_active` TINYINT UNSIGNED NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`))
DEFAULT CHARACTER SET = utf8;
