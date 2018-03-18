dotnet MergePhrase.dll -i s2t_phrase.json -o s2t_phrase_all.txt -l log_s2tphrase1.txt
dotnet MergePhrase.dll -i s2t_phrase1.txt -o s2t_phrase_all.txt -l log_s2tphrase2.txt
dotnet MergePhrase.dll -i s2t_phrase2.txt -o s2t_phrase_all.txt -l log_s2tphrase3.txt

dotnet MergePhrase.dll -i s2t_char.json -o s2t_char_all.txt -l log_s2tchar1.txt
dotnet MergePhrase.dll -i s2t_char1.txt -o s2t_char_all.txt -l log_s2tchar2.txt
dotnet MergePhrase.dll -i s2t_char2.txt -o s2t_char_all.txt -l log_s2tchar3.txt

dotnet MergePhrase.dll -i t2s_phrase1.txt -o t2s_phrase_all.txt -l log_t2sphrase.txt

dotnet MergePhrase.dll -i t2s_char.json -o t2s_char_all.txt -l log_t2schar1.txt
dotnet MergePhrase.dll -i t2s_char1.txt -o t2s_char_all.txt -l log_t2schar2.txt
dotnet MergePhrase.dll -i t2s_char2.txt -o t2s_char_all.txt -l log_t2schar3.txt
