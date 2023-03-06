# Trabajo Práctico Final
------------
#### Integrantes

- Alessandro Porcel
- Gian Franco Paglieri
- Erwin Quispe
- Cecilia Perez De San Roman
- Kevin Carballal

## RED SOCIAL

Desarrollo de un sitio web utilizando MVC Framework Core en combinacion con Entity Framework Core como ORM para la persistencia de datos.

### Caracteristicas

- Creacion de usuario.
- Login tanto para usuario, como para administrador.
- EL motor de base de datos que utiliza este proyecto es MySQL, así que tiene que tener un programa que permita correr MySQL, por ejemplo Xampp. Para poder conectar la bbdd a la aplicación lo que tiene que completar son los datos dentro del archivo Credentials.cs que se encuentra ubicadad dentro de la carpeta Helpers. En este archivo solamente tendrás que colocar los datos correspondientes a tu bbdd: nombre de la base, usuario y password, puerto de conexión.

![credencial](https://user-images.githubusercontent.com/70297930/223010264-48ba8638-92d5-4618-982d-1cd0f06389c9.jpg)

### **VISTA USUARIO**

Una vez que se loguea el usuario normal va a ser redireccionado a una pantalla Home que tendrá las siguientes características:
- Navbar en la que podrá ir a "Mis datos", volver al Home, o cerrar sesión.
- Buscador de posts, que busca por fecha, contenido, usuario o tags.
- Accesos directos para buscar amigos, ver a sus amigos actuales con la posibilidad de eliminarlos, y un último botón para ver los posts propios del usuario.
- Una sección para poder crear fácilmente posts propios que luego serán visualizados por sus amigos.
- Sección de listado de posts de los amigos del usuario logueado.
- En el home se encuentra una sección para poder ver los posts de tus amigos, en los cuales podrás reaccionar con distintos tipos de reacciones y también contar con la posiblidad de dejar comentarios.
- Dentro de la sección de "Mis Posts", se encuentran los posts creados por el mismo usuario, teniendo la posibilidad de crear rápidamente nuevos posts, y en los ya existentes, podrá eliminarlos o editarlos simplemente.
- En la sección "Mis datos", podrás ver tus datos actuales con la posibilidad de cambiarlos, incluso poder cambiar la contraseña, en donde tendrás que colocar la contraseña actual (como sistema de validación) y escribir dos veces la nueva contraseña

![IndexUsuario](https://user-images.githubusercontent.com/70297930/223012395-a5a4de1c-bdc7-47b1-a5d9-0dd2642980eb.jpg)

### **VISTA ADMINISTRADOR**
- Inicio
- ABM de usuarios
- ABM de posts
- ABM de tags
- ABM de comentarios
- Cerrar sesion

![IndexAdmin](https://user-images.githubusercontent.com/70297930/223013424-74db5885-8b6c-428b-9242-debbf51725ef.jpg)

### Tecnologias
- ASP .NET CORE
- C#

### Extras
- Mejoras a nivel visual (Uso de Bootstrap e iconos).
- Sonidos de confirmacion y error.
- Validacion de tiempo de sesion con las propiedades de Autenticación.
- Uso de peticiones API con javascript para la funcionalidad de reacciones.
