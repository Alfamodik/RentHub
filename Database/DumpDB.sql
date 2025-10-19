-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: 94.183.186.221    Database: rent_hub
-- ------------------------------------------------------
-- Server version	8.0.43-0ubuntu0.24.04.2

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `advertisements`
--

DROP TABLE IF EXISTS `advertisements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `advertisements`
--

LOCK TABLES `advertisements` WRITE;
/*!40000 ALTER TABLE `advertisements` DISABLE KEYS */;
/*!40000 ALTER TABLE `advertisements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `flats`
--

DROP TABLE IF EXISTS `flats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `flats`
--

LOCK TABLES `flats` WRITE;
/*!40000 ALTER TABLE `flats` DISABLE KEYS */;
/*!40000 ALTER TABLE `flats` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `placement_platforms`
--

DROP TABLE IF EXISTS `placement_platforms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `placement_platforms` (
  `platform_id` int NOT NULL AUTO_INCREMENT,
  `platform_name` varchar(100) NOT NULL,
  PRIMARY KEY (`platform_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `placement_platforms`
--

LOCK TABLES `placement_platforms` WRITE;
/*!40000 ALTER TABLE `placement_platforms` DISABLE KEYS */;
/*!40000 ALTER TABLE `placement_platforms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `renters`
--

DROP TABLE IF EXISTS `renters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `renters` (
  `renter_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(60) NOT NULL,
  `lastname` varchar(60) NOT NULL,
  `patronymic` varchar(60) DEFAULT NULL,
  `phone_number` varchar(16) NOT NULL COMMENT '+7 900 304 93 12',
  PRIMARY KEY (`renter_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `renters`
--

LOCK TABLES `renters` WRITE;
/*!40000 ALTER TABLE `renters` DISABLE KEYS */;
/*!40000 ALTER TABLE `renters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reservations`
--

DROP TABLE IF EXISTS `reservations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reservations`
--

LOCK TABLES `reservations` WRITE;
/*!40000 ALTER TABLE `reservations` DISABLE KEYS */;
/*!40000 ALTER TABLE `reservations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `email` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'string','string');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-10-19 23:57:10
