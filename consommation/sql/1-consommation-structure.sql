-- Base de données : consommation
-- Script 1/2 : structure (base, utilisateur, tables, index, contraintes)

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";
SET NAMES utf8mb4;

DROP DATABASE IF EXISTS `consommation`;
CREATE DATABASE `consommation` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `consommation`;

--
-- Utilisateur : `u_conso`
-- Mot de passe : `MDP` (à remplacer par votre propre mot de passe si besoin)
--
DROP USER IF EXISTS 'u_conso'@'localhost';
CREATE USER 'u_conso'@'localhost' IDENTIFIED BY 'MDP';
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON `consommation`.* TO 'u_conso'@'localhost';

-- --------------------------------------------------------

--
-- Structure de la table `appareil`
--

CREATE TABLE `appareil` (
  `id` int NOT NULL,
  `denomination` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ref_type` int NOT NULL,
  `ref_zone` int NOT NULL,
  `quantite` smallint NOT NULL,
  `puissance` decimal(10,3) DEFAULT NULL,
  `consommation` decimal(10,6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Structure de la table `type_appareil`
--

CREATE TABLE `type_appareil` (
  `id` int NOT NULL,
  `nom` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `commentaire` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `facteur_de_marche` double NOT NULL,
  `puissance` decimal(10,3) DEFAULT NULL,
  `consommation` decimal(10,6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Structure de la table `utilisation`
--

CREATE TABLE `utilisation` (
  `id` int NOT NULL,
  `ref_appareil` int NOT NULL,
  `quantite` smallint NOT NULL,
  `pourcentage` double NOT NULL,
  `debut_periode` datetime NOT NULL,
  `fin_periode` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Structure de la table `zone`
--

CREATE TABLE `zone` (
  `id` int NOT NULL,
  `nom` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `commentaire` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Index pour les tables déchargées
--

--
-- Index pour la table `appareil`
--
ALTER TABLE `appareil`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ref_type` (`ref_type`),
  ADD KEY `ref_zone` (`ref_zone`);

--
-- Index pour la table `type_appareil`
--
ALTER TABLE `type_appareil`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nom` (`nom`);

--
-- Index pour la table `utilisation`
--
ALTER TABLE `utilisation`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ref_appareil` (`ref_appareil`);

--
-- Index pour la table `zone`
--
ALTER TABLE `zone`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nom` (`nom`);

--
-- AUTO_INCREMENT pour les tables déchargées
--

--
-- AUTO_INCREMENT pour la table `appareil`
--
ALTER TABLE `appareil`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `type_appareil`
--
ALTER TABLE `type_appareil`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `utilisation`
--
ALTER TABLE `utilisation`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `zone`
--
ALTER TABLE `zone`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- Contraintes pour les tables déchargées
--

--
-- Contraintes pour la table `appareil`
--
ALTER TABLE `appareil`
  ADD CONSTRAINT `appareil_ibfk_1` FOREIGN KEY (`ref_type`) REFERENCES `type_appareil` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `appareil_ibfk_2` FOREIGN KEY (`ref_zone`) REFERENCES `zone` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Contraintes pour la table `utilisation`
--
ALTER TABLE `utilisation`
  ADD CONSTRAINT `utilisation_ibfk_1` FOREIGN KEY (`ref_appareil`) REFERENCES `appareil` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

COMMIT;
