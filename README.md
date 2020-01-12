---------------------------------Ru-----------------------------------

Это решение содержит 4 проекта - три независимых приложения и слой, связывающий каждое из них с библиотекой OpenCVSharp v2.4.

1) BloodAnalysis. Проект, анализирующий снимок крови человека и разбивающий найденные там элементы на три группы (группа интереса - лейкоциты, мелкие объекты - зачастую это тромбоциты и всё остальное). Доступен функционал специальной обработки изображения и фильтрации обнаруженных объектов. Pet-project.

2) LeafArea. Программа, позволяющая измерить площадь сложных объектов на фотографии. Важным условием является наличие на изображении эталонного объекта, площадь которого уже известна, а так же общая высокая контрастность фотографии. Доступен функционал специальной обработки изображения. Можно работать в двух режимах - все объекты за раз (автоматический поиск) и вручную. Изначальная цель разработки - анализ площади листьев растений. Диплом бакалавра.

3) OpenMaps. Приложение для построения маршрута полёта БПЛА на основе снимков с его борта и общей карты местности. Входными данными являются общая карта местности и набор снимков, именованных в порядке создания (Фото1, Фото2 ... ФотоN). На выходе получаем отрисованный маршрут и (при необходимости) скорость полёта БПЛА и покрытие местности снимками с меньшей высоты. Доступна настройка слоёв отрисовки. Ввиду высокой требовательности используемого алгоритма (ASIFT) программа работает в фоновом режиме (1 поток). Диплом магистра.

4) Sharedlogic. Слой, связывающий три вышеперечисленных проекта с библиотекой OpenCVSharp. Ввиду повторного использования функционала, было решено вынести всё это в отдельную библиотеку.

---------------------------------EN-----------------------------------

This solution contains 4 projects - three independent applications and a layer connecting each of them with the library OpenCVSharp v2.4.

1) BloodAnalysis. A project that analyzes a person’s blood picture and breaks down the elements found there into three groups (the 'interest' group is white blood cells, small objects are often platelets and everything else). The functionality of special image processing and filtering of detected objects is available. Pet-project.

2) LeafArea. A program that allows you to measure the area of complex objects on the photo. An important condition is the presence in the image of an 'etalon' object, the area of which is already known, as well as the overall high contrast of the photo. Special image processing functionality is available. You can work in two modes - all objects at a time (automatic search) and manually. The initial development goal is to analyze the area of plant leaves. Bachelor's diploma.

3) OpenMaps. An application for constructing a UAV flight route based on images from its board and a general terrain map. The input data is a general map of the area and a set of images named in the order of creation (Photo1, Photo2 ... PhotoN). At the exit, we get the rendered route and (if necessary and if available time of creating of each single photo) the UAV flight speed and the terrain coverage with images from a lower height. Customization of rendering layers is available. Due to the high demands on the algorithm used (ASIFT), the program runs in the background (1 thread). Master's degree.

4) SharedLogic. A layer linking the three above projects with the library OpenCVSharp. Due to the reuse of functionality, it was decided to put all this in a separate library.
