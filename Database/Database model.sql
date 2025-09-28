CREATE SCHEMA `rent_hub`;
USE `rent_hub`;

CREATE TABLE `advertisements` (
  `advertisement_id` int NOT NULL AUTO_INCREMENT,
  `flat_id` int NOT NULL,
  `platform_id` int NOT NULL,
  `rent_type` enum('Посуточно','Длительный период') NOT NULL,
  `price_for_period` double NOT NULL,
  `income_for_period` double NOT NULL,
  `link_to_advertisement` longtext NOT NULL,
  PRIMARY KEY (`advertisement_id`),
  KEY `fk_flat_id_idx` (`flat_id`),
  KEY `fk_platform_id_idx` (`platform_id`),
  CONSTRAINT `fk_flat_id` FOREIGN KEY (`flat_id`) REFERENCES `flats` (`flat_id`),
  CONSTRAINT `fk_platform_id` FOREIGN KEY (`platform_id`) REFERENCES `placement_platforms` (`platform_id`)
);

CREATE TABLE `flats` (
  `flat_id` int NOT NULL AUTO_INCREMENT,
  `country` varchar(100) NOT NULL,
  `city` varchar(100) NOT NULL,
  `district` varchar(100) NOT NULL,
  `house_number` varchar(10) NOT NULL,
  `room_count` int NOT NULL,
  `size` decimal(6,2) NOT NULL,
  `floor_number` int NOT NULL,
  `floors_number` int DEFAULT NULL,
  `description` text NOT NULL,
  `photo` blob,
  PRIMARY KEY (`flat_id`)
);

CREATE TABLE `placement_platforms` (
  `platform_id` int NOT NULL AUTO_INCREMENT,
  `platform_name` varchar(100) NOT NULL,
  PRIMARY KEY (`platform_id`)
);

CREATE TABLE `renters` (
  `renter_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(60) NOT NULL,
  `lastname` varchar(60) NOT NULL,
  `patronymic` varchar(60) DEFAULT NULL,
  `phone_number` varchar(16) NOT NULL COMMENT '+7 900 304 93 12',
  PRIMARY KEY (`renter_id`)
);

CREATE TABLE `reservations` (
  `reservation_id` int NOT NULL AUTO_INCREMENT,
  `advertisement_id` int NOT NULL,
  `renter_id` int NOT NULL,
  `date_of_start_reservation` date NOT NULL,
  `date_of_end_reservation` date NOT NULL,
  `summ` decimal(10,2) NOT NULL,
  `income` decimal(10,2) NOT NULL,
  PRIMARY KEY (`reservation_id`),
  KEY `fk_ad_id_idx` (`advertisement_id`),
  KEY `fk_renter_id_idx` (`renter_id`),
  CONSTRAINT `fk_ad_id` FOREIGN KEY (`advertisement_id`) REFERENCES `advertisements` (`advertisement_id`),
  CONSTRAINT `fk_renter_id` FOREIGN KEY (`renter_id`) REFERENCES `renters` (`renter_id`)
);

CREATE TABLE `users`(
	`user_id` INT AUTO_INCREMENT,
    `email` VARCHAR(100) NOT NULL,
    `password` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`user_id`)
);