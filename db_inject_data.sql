USE {DATABASE_NAME}
INSERT INTO filters (recordId, filter, description, title) VALUES (1, 'DEFAULT', 'list of default fail words', 'default');
INSERT INTO filters (recordId, filter, description, title) VALUES (2, 'TEST', 'Test filter with longer desc.', 'Test');

INSERT INTO words (recordId, word) VALUES (1, 'crap');
INSERT INTO words (recordId, word) VALUES (2, 'stupid');
INSERT INTO words (recordId, word) VALUES (3, 'nimrod');
INSERT INTO words (recordId, word) VALUES (4, 'nincompoop');
INSERT INTO words (recordId, word) VALUES (5, 'poop');
INSERT INTO words (recordId, word) VALUES (6, 'sucks');
INSERT INTO words (recordId, word) VALUES (7, 'bill gates');
INSERT INTO words (recordId, word) VALUES (8, 'kill');
INSERT INTO words (recordId, word) VALUES (9, 'killer');
INSERT INTO words (recordId, word) VALUES (10, 'wonder bread');
INSERT INTO words (recordId, word) VALUES (11, 'crabapple');
INSERT INTO words (recordId, word) VALUES (12, 'crud');
INSERT INTO words (recordId, word) VALUES (13, 'fart');
INSERT INTO words (recordId, word) VALUES (14, 'crappy');
INSERT INTO words (recordId, word) VALUES (15, 'crack');
INSERT INTO words (recordId, word) VALUES (16, 'cracker');
INSERT INTO words (recordId, word) VALUES (17, 'copulate');
INSERT INTO words (recordId, word) VALUES (18, 'sixtynine');
INSERT INTO words (recordId, word) VALUES (19, 'shoot');
INSERT INTO words (recordId, word) VALUES (20, 'premature');


INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 1);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 2);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 3);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 4);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 5);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 6);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 7);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 8);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 9);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 10);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 11);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 12);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 13);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 14);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 15);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 16);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 17);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 18);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 19);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (1, 20);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (2, 16);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (2, 7);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (2, 10);
INSERT INTO filter_words (filterRecordId, wordRecordId) VALUES (2, 19);

