-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Hôte : localhost
-- Généré le : jeu. 23 jan. 2025 à 14:46
-- Version du serveur : 8.0.31
-- Version de PHP : 8.0.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données : `multimedia`
--

USE multimedia;

--
-- Nettoyage des données existantes
--

DELETE FROM detail;
DELETE FROM liste;
DELETE FROM media;
DELETE FROM categorie;

--
-- Déchargement des données de la table `categorie`
--

INSERT INTO `categorie` (`id`, `nom`) VALUES
(8, 'Chanson Française 60'),
(9, 'Chanson Française 70'),
(7, 'Clip Video'),
(4, 'Musique Disco'),
(5, 'Musique Pop 80'),
(6, 'Musique Pop 90'),
(1, 'Musique Rock 60'),
(2, 'Musique Rock 70'),
(3, 'Musique Rock 80');

--
-- Déchargement des données de la table `media`
--

INSERT INTO `media` (`id`, `titre`, `nomfichier`, `duree`, `ref_categorie`) VALUES
(1, 'Run Through The Jungle (Creedence Clearwater Revival)', 'runjungle.mp3', '00:03:07', 2),
(2, 'Music From Vietnam War (youtube)', 'https://www.youtube.com/watch?v=sDcRCHXQ9gs', '01:02:56', 2),
(3, 'Liar, Liar (The Castaways)', 'https://www.youtube.com/watch?v=IH8Fb-_jmTA', '00:02:27', 1),
(7, 'Go Your Own Way (Fleetwood Mac)', 'YourOwnWay.mp3', '00:03:36', 2),
(8, 'A Horse With No Name (America)', 'HorseNoName.mp3', '00:04:07', 2),
(9, 'Porcelain (Moby)', 'Porcelain.mp3', '00:04:01', 6),
(10, 'Don\'t You / Forget About Me (Simple Minds)', 'forgetaboutme.mp3', '00:06:45', 3),
(11, 'The Logical Song (Supertramp)', 'logicalsong.mp3', '00:04:08', 2),
(12, 'Little Bird (Annie Lennox)', 'littlebird.mp3', '00:04:49', 6),
(13, 'Madame Rêve (Alain Bashung)', 'madamereve.mp3', '00:04:50', 6),
(14, 'In Your Room (Depeche Mode)', 'inyourroom.mp3', '00:06:27', 6),
(15, 'Luka (Suzanne Vega)', 'lukavegas.mp3', '00:03:51', 5),
(16, 'Le poinçonneur des lilas (Serge Gainsbourg)', 'lilas.mp3', '00:02:42', 8),
(17, 'Je Suis Venu Te Dire Que Je M\'en Vais (Serge Gainsbourg)', 'jesuisvenu.mp3', '00:03:23', 9);

--
-- Déchargement des données de la table `liste`
--

INSERT INTO `liste` (`id`, `nom`) VALUES
(2, 'Ambiance Bloody'),
(1, 'Un peu de tout');

--
-- Déchargement des données de la table `detail`
--

INSERT INTO `detail` (`id`, `ref_liste`, `ordre`, `ref_media`) VALUES
(1, 1, 3, 7),
(2, 1, 1, 15),
(3, 1, 2, 11),
(4, 2, 2, 1),
(5, 2, 1, 2),
(6, 2, 4, 16),
(7, 2, 3, 3);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
