-- Base de données : quetes
-- Script 1/2 : structure (base, utilisateur, tables, index, contraintes)

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";
SET NAMES utf8mb4;

DROP DATABASE IF EXISTS `quetes`;
CREATE DATABASE `quetes` DEFAULT CHARACTER SET latin1;
USE `quetes`;

--
-- Utilisateur : `u_quetes`
-- Mot de passe : `MDP` (à remplacer par votre propre mot de passe si besoin)
--
DROP USER IF EXISTS 'u_quetes'@'localhost';
CREATE USER 'u_quetes'@'localhost' IDENTIFIED BY 'MDP';
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON `quetes`.* TO 'u_quetes'@'localhost';

-- --------------------------------------------------------

--
-- Structure de la table `caracteristique`
--

CREATE TABLE `caracteristique` (
  `id` int NOT NULL,
  `nom` varchar(80) NOT NULL,
  `type` char(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `entite`
--

CREATE TABLE `entite` (
  `id` int NOT NULL,
  `nom` varchar(240) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `quete`
--

CREATE TABLE `quete` (
  `id` int NOT NULL,
  `nom` varchar(240) NOT NULL,
  `description` text NOT NULL,
  `ref_region` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `quete_detail`
--

CREATE TABLE `quete_detail` (
  `id` int NOT NULL,
  `ref_quete` int NOT NULL,
  `ref_caracteristique` int NOT NULL,
  `valeur` varchar(240) NOT NULL,
  `ref_entite` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `region`
--

CREATE TABLE `region` (
  `id` int NOT NULL,
  `nom` varchar(120) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Index pour les tables déchargées
--

--
-- Index pour la table `caracteristique`
--
ALTER TABLE `caracteristique`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nom` (`nom`);

--
-- Index pour la table `entite`
--
ALTER TABLE `entite`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nom` (`nom`);

--
-- Index pour la table `quete`
--
ALTER TABLE `quete`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nom` (`nom`),
  ADD KEY `ref_region` (`ref_region`);

--
-- Index pour la table `quete_detail`
--
ALTER TABLE `quete_detail`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ref_quete` (`ref_quete`),
  ADD KEY `ref_caracteristique` (`ref_caracteristique`),
  ADD KEY `ref_entite` (`ref_entite`);

--
-- Index pour la table `region`
--
ALTER TABLE `region`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nom` (`nom`);

--
-- AUTO_INCREMENT pour les tables déchargées
--

--
-- AUTO_INCREMENT pour la table `caracteristique`
--
ALTER TABLE `caracteristique`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `entite`
--
ALTER TABLE `entite`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `quete`
--
ALTER TABLE `quete`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `quete_detail`
--
ALTER TABLE `quete_detail`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `region`
--
ALTER TABLE `region`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- Contraintes pour les tables déchargées
--

--
-- Contraintes pour la table `quete`
--
ALTER TABLE `quete`
  ADD CONSTRAINT `quete_ibfk_1` FOREIGN KEY (`ref_region`) REFERENCES `region` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Contraintes pour la table `quete_detail`
--
ALTER TABLE `quete_detail`
  ADD CONSTRAINT `quete_detail_ibfk_1` FOREIGN KEY (`ref_quete`) REFERENCES `quete` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `quete_detail_ibfk_2` FOREIGN KEY (`ref_caracteristique`) REFERENCES `caracteristique` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `quete_detail_ibfk_3` FOREIGN KEY (`ref_entite`) REFERENCES `entite` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

COMMIT;
