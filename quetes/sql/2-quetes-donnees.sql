-- Base de données : quetes
-- Script 2/2 : données de démonstration (à exécuter après le script de structure)

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";
SET NAMES utf8mb4;

USE `quetes`;

--
-- Données de la table `region`
--

INSERT INTO `region` (`id`, `nom`) VALUES
(1, 'Calme comté'),
(2, 'La Forêt des âmes en peine'),
(3, 'Mont des rapaces'),
(4, 'Sombre-Monts');

--
-- Données de la table `caracteristique`
--

INSERT INTO `caracteristique` (`id`, `nom`, `type`) VALUES
(1, 'Donneur de quête', 'Q'),
(2, 'Récompense financière', 'M'),
(3, 'Valideur de quête', 'Q'),
(4, 'Récompense matérielle possible', 'Q'),
(5, 'Conseils', 'T');

--
-- Données de la table `entite`
--

INSERT INTO `entite` (`id`, `nom`) VALUES
(4, 'Bandage de soin instantané contre les plaies purulentes'),
(3, 'Bâton du crépuscule (bâton 9,3 dps)'),
(2, 'Couteau de boucher (dague 8,9 dps)'),
(1, 'Irina Stamgood');

--
-- Données de la table `quete`
--

INSERT INTO `quete` (`id`, `nom`, `description`, `ref_region`) VALUES
(1, 'L\'étrange silhouette', 'Une étrange silhouette a été vue par maints voyageurs rodant aux abords de la colline aux corbeaux ...', 4);

--
-- Données de la table `quete_detail`
--

INSERT INTO `quete_detail` (`id`, `ref_quete`, `ref_caracteristique`, `valeur`, `ref_entite`) VALUES
(1, 1, 1, '1', 1),
(2, 1, 2, '35', NULL),
(3, 1, 3, '1', 1),
(4, 1, 4, '1', 2),
(5, 1, 4, '1', 3),
(6, 1, 4, '5', 4),
(7, 1, 5, 'Ne partez pas seul !', NULL);

COMMIT;
