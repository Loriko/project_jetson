-- -----------------------------------------------------
-- Schema jetson
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `jetson` ;

CREATE SCHEMA IF NOT EXISTS `jetson` DEFAULT CHARACTER SET utf8 ;
USE `jetson` ;

-- -----------------------------------------------------
-- Table `jetson`.`user`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`user` ;

CREATE TABLE IF NOT EXISTS `jetson`.`user` (
  `id` INT(5) UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(45) NOT NULL,
  `password` VARCHAR(45) NOT NULL,
  `email_address` VARCHAR(45) NULL,
  `first_name` VARCHAR(45) NULL,
  `last_name` VARCHAR(45) NULL,
  `password_reset_token` VARCHAR(100) NULL,
  `is_administrator` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `username_UNIQUE` (`username` ASC))
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`location`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`location` ;

CREATE TABLE IF NOT EXISTS `jetson`.`location` (
  `id` INT(5) UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT(5) UNSIGNED NOT NULL,
  `location_name` VARCHAR(45) NOT NULL,
  `address_line` VARCHAR(200) NULL DEFAULT NULL,
  `city` VARCHAR(45) NULL DEFAULT NULL,
  `state` VARCHAR(45) NULL DEFAULT NULL,
  `zip` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_location_has_user_user`
    FOREIGN KEY (`user_id`)
    REFERENCES `jetson`.`user` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`room`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`room` ;

CREATE TABLE IF NOT EXISTS `jetson`.`room` (
  `id` INT(5) UNSIGNED NOT NULL AUTO_INCREMENT,
  `location_id` INT(5) UNSIGNED NULL DEFAULT NULL,
  `room_name` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_room_Address`
    FOREIGN KEY (`location_id`)
    REFERENCES `jetson`.`location` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`camera`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`camera` ;

CREATE TABLE IF NOT EXISTS `jetson`.`camera` (
  `id` INT(5) UNSIGNED NOT NULL AUTO_INCREMENT,
  `camera_name` VARCHAR(45) NULL DEFAULT NULL,
  `camera_key` VARCHAR(12) NOT NULL UNIQUE,
  `location_id` INT(5) UNSIGNED NULL DEFAULT NULL,
  `room_id` INT(5) UNSIGNED NULL DEFAULT NULL,
  `user_id` INT(5) UNSIGNED NULL DEFAULT NULL,
  `monitored_area` VARCHAR(45) NULL DEFAULT NULL,
  `brand` VARCHAR(45) NULL DEFAULT NULL,
  `model` VARCHAR(45) NULL DEFAULT NULL,
  `resolution` VARCHAR(45) NULL DEFAULT NULL,
  `image_path` VARCHAR(150) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_camera_Address`
    FOREIGN KEY (`location_id`)
    REFERENCES `jetson`.`location` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_camera_Room`
    FOREIGN KEY (`room_id`)
    REFERENCES `jetson`.`room` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`per_hour_stat`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`per_hour_stat` ;

CREATE TABLE IF NOT EXISTS `jetson`.`per_hour_stat` (
  `id` INT(8) UNSIGNED NOT NULL AUTO_INCREMENT,
  `camera_id` INT(5) UNSIGNED NOT NULL,
  `date_day` DATE NOT NULL,
  `date_hour` INT(2) UNSIGNED NOT NULL,
  `max_detected_object` INT(5) UNSIGNED NOT NULL,
  `min_detected_object` INT(5) UNSIGNED NOT NULL,
  `avg_detected_object` FLOAT(5,3) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`))
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`per_second_stat`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`per_second_stat` ;

CREATE TABLE IF NOT EXISTS `jetson`.`per_second_stat` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `camera_id` INT(5) UNSIGNED NOT NULL,
  `num_detected_object` INT(5) UNSIGNED NOT NULL,
  `date_time` DATETIME NOT NULL,
  `has_saved_image` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `per_hour_stat_id` INT(8) UNSIGNED NULL DEFAULT NULL,
  `frm_jpg_path` VARCHAR(150) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `idStat_UNIQUE` (`id` ASC),
  CONSTRAINT `fk_per_second_stat_camera`
    FOREIGN KEY (`camera_id`)
    REFERENCES `jetson`.`camera` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`user_camera_association`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`user_camera_association` ;

CREATE TABLE IF NOT EXISTS `jetson`.`user_camera_association` (
  `camera_id` INT(5) UNSIGNED NOT NULL,
  `user_id` INT(5) UNSIGNED NOT NULL,
  PRIMARY KEY (`camera_id`, `user_id`),
  CONSTRAINT `fk_camera_has_user_camera`
    FOREIGN KEY (`camera_id`)
    REFERENCES `jetson`.`camera` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE,
  CONSTRAINT `fk_camera_has_user_user`
    FOREIGN KEY (`user_id`)
    REFERENCES `jetson`.`user` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`api_key`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`api_key` ;

CREATE TABLE IF NOT EXISTS `jetson`.`api_key` (
  `id` INT(5) UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT(5) UNSIGNED NOT NULL,
  `api_key` CHAR(32) NOT NULL,
  `salt` VARCHAR(24) NOT NULL,
  `is_active` TINYINT UNSIGNED NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_apikey_has_user`
    FOREIGN KEY (`user_id`)
    REFERENCES `jetson`.`user` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`alert`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`alert` ;

CREATE TABLE IF NOT EXISTS `jetson`.`alert` (
  `id` INT(5) UNSIGNED NOT NULL AUTO_INCREMENT,
  `alert_name` VARCHAR(100) NOT NULL,
  `camera_id` INT(5) UNSIGNED NOT NULL,
  `user_id` INT(5) UNSIGNED NOT NULL,
  `contact_method` VARCHAR(30) NOT NULL,
  `trigger_operator` VARCHAR(30) NOT NULL,
  `trigger_number` INT(5) UNSIGNED NOT NULL,
  `always_active` TINYINT NOT NULL,
  `start_time` TIME NULL,
  `end_time` TIME NULL,
--   `trigger_count` INT(5) NOT NULL DEFAULT 0,
  `disabled_until` DATETIME NULL,
  `snoozed_until` DATETIME NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_alert_has_camera`
    FOREIGN KEY (`camera_id`)
    REFERENCES `jetson`.`camera` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE ,
  CONSTRAINT `fk_fk_alert_has_user`
    FOREIGN KEY (`user_id`)
    REFERENCES `jetson`.`user` (`id`)
    ON DELETE CASCADE 
    ON UPDATE CASCADE)
DEFAULT CHARACTER SET = utf8;

-- -----------------------------------------------------
-- Table `jetson`.`notification`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `jetson`.`notification` ;

CREATE TABLE IF NOT EXISTS `jetson`.`notification` (
  `id` INT(5) UNSIGNED NOT NULL AUTO_INCREMENT,
  `alert_id` INT(5) UNSIGNED NOT NULL,
  `trigger_datetime` DATETIME NOT NULL,
  `acknowledged` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `failed_email` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_notification_has_alert`
    FOREIGN KEY (`alert_id`)
    REFERENCES `jetson`.`alert` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
DEFAULT CHARACTER SET = utf8;