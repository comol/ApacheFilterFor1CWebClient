Приложение предназначено для модификации интерфейса WEB клиента 1С на уровне сервера.
Что можно сделать при помощи него:
- Вставить Яндекс метрику или Google Аналитику
- Убрать лишнее (заголовки, кнопки, заставки, рекламу)
- поменять цветовую схему

Использование:
ExtFilterDefine fixsplash intype=text/html mode=output cmd="D:/NoSplashFor1C.exe D:/findstring.txt D:/replacestring.txt"


<Directory "...">
	
  .........................
  SetOutputFilter fixsplash
	
</Directory>

В первом текстовом файле содержится текст для поиска, во втором - для замены.
