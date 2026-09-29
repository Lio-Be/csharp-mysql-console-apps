-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Hôte : localhost
-- Généré le : lun. 20 jan. 2025 à 14:56
-- Version du serveur : 8.0.31
-- Version de PHP : 8.0.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

-- --------------------------------------------------------

--
-- Base de données : multimedia
--
DROP DATABASE IF EXISTS multimedia;
CREATE DATABASE IF NOT EXISTS multimedia DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE multimedia;

-- --------------------------------------------------------

--
-- Utilisateur : `u_multimedia`
-- Mot de passe : `MDP` (à remplacer par votre propre mot de passe si besoin)
--
DROP USER IF EXISTS 'u_multimedia'@'localhost';
CREATE USER 'u_multimedia'@'localhost' IDENTIFIED WITH caching_sha2_password BY 'MDP';
GRANT USAGE ON *.* TO 'u_multimedia'@'localhost';
ALTER USER 'u_multimedia'@'localhost' REQUIRE NONE WITH MAX_QUERIES_PER_HOUR 0 MAX_CONNECTIONS_PER_HOUR 0 MAX_UPDATES_PER_HOUR 0 MAX_USER_CONNECTIONS 0;
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON multimedia.* TO 'u_multimedia'@'localhost';
ALTER USER 'u_multimedia'@'localhost';

-- --------------------------------------------------------

--
-- Structure de la table categorie
--

CREATE TABLE categorie (
  id int NOT NULL,
  nom varchar(60) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Structure de la table detail
--

CREATE TABLE detail (
  id int NOT NULL,
  ref_liste int NOT NULL,
  ordre smallint NOT NULL,
  ref_media int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Structure de la table liste
--

CREATE TABLE liste (
  id int NOT NULL,
  nom varchar(60) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Structure de la table media
--

CREATE TABLE media (
  id int NOT NULL,
  titre varchar(120) NOT NULL,
  nomfichier varchar(1000) NOT NULL,
  duree time NOT NULL,
  ref_categorie int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Index pour les tables déchargées
--

--
-- Index pour la table categorie
--
ALTER TABLE categorie
  ADD PRIMARY KEY (id),
  ADD UNIQUE KEY nom (nom);

--
-- Index pour la table detail
--
ALTER TABLE detail
  ADD PRIMARY KEY (id),
  ADD UNIQUE KEY unicite__ref_liste__ordre (ref_liste,ordre),
  ADD KEY ref_media (ref_media),
  ADD KEY ref_liste (ref_liste) USING BTREE;

--
-- Index pour la table liste
--
ALTER TABLE liste
  ADD PRIMARY KEY (id),
  ADD UNIQUE KEY nom (nom);

--
-- Index pour la table media
--
ALTER TABLE media
  ADD PRIMARY KEY (id),
  ADD UNIQUE KEY titre (titre),
  ADD KEY ref_categorie (ref_categorie);

--
-- AUTO_INCREMENT pour les tables déchargées
--

--
-- AUTO_INCREMENT pour la table categorie
--
ALTER TABLE categorie
  MODIFY id int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table detail
--
ALTER TABLE detail
  MODIFY id int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table liste
--
ALTER TABLE liste
  MODIFY id int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table media
--
ALTER TABLE media
  MODIFY id int NOT NULL AUTO_INCREMENT;

--
-- Contraintes pour les tables déchargées
--

--
-- Contraintes pour la table detail
--
ALTER TABLE detail
  ADD CONSTRAINT detail_ibfk_1 FOREIGN KEY (ref_liste) REFERENCES liste (id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT detail_ibfk_2 FOREIGN KEY (ref_media) REFERENCES media (id) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Contraintes pour la table media
--
ALTER TABLE media
  ADD CONSTRAINT media_ibfk_1 FOREIGN KEY (ref_categorie) REFERENCES categorie (id) ON DELETE RESTRICT ON UPDATE RESTRICT;
COMMIT;
