using LecturaFicheros;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UserSearch
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Contacto> contactos = new ObservableCollection<Contacto>();
        ObservableCollection<Contacto> busquedaContactos = new ObservableCollection<Contacto>();
        ObservableCollection<Contacto> emptyList = new ObservableCollection<Contacto>();
        MatchCollection matches;

        bool dataHasBeenLoaded = false;

        public MainPage()
        {
            InitializeComponent();

            examineButton.Clicked += (sender, args) =>
            {
                if (!dataHasBeenLoaded) { 
                readandInsertContacts();
                idListaContacto.ItemsSource = contactos;
                dataHasBeenLoaded = true;
                }
                else
                {
                    DisplayAlert("Error", "El fichero ya se encuentra cargado", "Aceptar");
                }
            };

            searchButton.Clicked += (sender, args) =>
            {
                if (!dataHasBeenLoaded)
                {
                    DisplayAlert("Error", "Ningún fichero ha sido cargado", "Aceptar");
                }
                else
                {
                    buscar();
                }
            };
        }

        public void readandInsertContacts()
        {
            int counter = 0;
            string line;
            Contacto contacto;
            string nombre = "";
            int edad = 0;
            string nif = "";
            string ruta = "UserSearch.Assets.Info.txt";
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(ruta);


            //Leer el fichero y mostrarlo línea a línea.
            System.IO.StreamReader fichero = new System.IO.StreamReader(stream);

            /*Bucle encargado de leer el fichero, ir almacenando los valores correspondientes
             a nombre, edad y nif e ir almacenándolos en un nuevo contacto.*/
            while ((line = fichero.ReadLine()) != null)
            {
                counter++;

                switch (counter)
                {
                    case 1:
                        nombre = line;
                        break;
                    case 2:
                        int.TryParse(line, out edad);
                        break;
                    case 3:
                        nif = line;
                        contacto = new Contacto(nombre, edad, nif);
                        contactos.Add(contacto);
                        counter = 0;
                        break;
                }
            }
        }

        /// <summary>
        /// Metodo encargado de realizar la busqueda, teniendo en cuenta los diferentes patrones
        /// de busqueda y tratando de controlar todos los posibles fallos.
        /// </summary>
        private void buscar()
        {

            int edad1, edad2;

            /*En Xamarin por lo visto si se deja un campo vacío lo da por nulo, por lo que 
             las comprobaciones de abajo con Equals nos darían un NullPointer, por lo que
             antes de buscar debemos de darle valor al texto del campo de la edad minima y maxima
             por si se dejan en blanco.*/
            if(txtMaxEdad.Text == null && minimumAgeBox.Text == null)
            {
                txtMaxEdad.Text = "";
                minimumAgeBox.Text = "";
            }else if(txtMaxEdad.Text == null && minimumAgeBox.Text != null)
            {
                txtMaxEdad.Text = "";
            }else if(txtMaxEdad.Text != null && minimumAgeBox.Text == null)
            {
                minimumAgeBox.Text = "";
            }else if(searchNameBox.Text == null)
            {
                searchNameBox.Text = "";
            }
            
            //Si no hay campos vacios
            if (!txtMaxEdad.Text.Equals("") && !minimumAgeBox.Text.Equals("") && !searchNameBox.Text.Equals(""))
            {
                //Se hace control numerico
                controlNumerico();
            }

            else if (!txtMaxEdad.Text.Equals("") && !minimumAgeBox.Text.Equals(""))
            {
                //Se hace control numerico
                controlNumerico();
            }
            else if (!txtMaxEdad.Text.Equals(""))
            {
                if (int.TryParse(txtMaxEdad.Text.Trim(), out edad1))
                {
                    //Se hace control numerico
                    realizarBusqueda();
                }
                else
                {
                    txtMaxEdad.Text = minimumAgeBox.Text = "";
                    DisplayAlert("Error", "Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.", "Aceptar");
                }
            }
            else if (!minimumAgeBox.Text.Equals(""))
            {
                if (int.TryParse(minimumAgeBox.Text.Trim(), out edad1))
                {
                    //Se hace control numerico
                    realizarBusqueda();
                }
                else
                {
                    txtMaxEdad.Text = minimumAgeBox.Text = "";
                    DisplayAlert("Error", "Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.", "Aceptar");

                }
            }
            else
            {
                realizarBusqueda();
            }
        }

        private void controlNumerico()
        {
            int edad1, edad2;

            if (int.TryParse(txtMaxEdad.Text.Trim(), out edad1) && int.TryParse(minimumAgeBox.Text.Trim(), out edad2))
            {
                if (int.Parse(minimumAgeBox.Text.ToString()) >= int.Parse(txtMaxEdad.Text.ToString()))
                {
                    DisplayAlert("Error", "La edad mínima no puede ser mayor o igual a la máxima.", "Aceptar");
                }
                else
                {
                    realizarBusqueda();
                }
            }
            else
            {
                txtMaxEdad.Text = minimumAgeBox.Text = "";
                DisplayAlert("Error", "Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.", "Aceptar");
            }
        }

        /// <summary>
        /// Realiza la busqueda de verdad
        /// </summary>
        private void realizarBusqueda()
        {
            //Limpiamos los contactos cargados para volver a cargar los correctos
            busquedaContactos.Clear();
            //Se obtiene resultado de la busqueda con los valores introducidos y se carga en listView
            buscarContacto(searchNameBox.Text);
            //Se rellena listView
            cargarListView();
        }

        /// <summary>
        /// Rellena el listVew con los contactos que tiene la lista "contactos"
        /// </summary>
        private void cargarListView()
        {
            idListaContacto.ItemsSource = busquedaContactos;
        }

        /// <summary>
        /// Implementa los filtros para obtener una list de contactos a mostrar
        /// </summary>
        /// <param name="filtro"></param>
        public void buscarContacto(string filtro)
        {
            /// Recorremos el array contactos y si encontramos una coincidencia la añadimos al arraylist resultado
            for (int i = 0; i < contactos.Count; i++)
            {
                if (comprobarNombre(contactos[i], filtro))
                {
                    busquedaContactos.Add(contactos[i]);
                }
            }

            //Si no se encontro ninguna coincidencia se informa
            if (busquedaContactos.Count == 0) {
                DisplayAlert("Fin de la busqueda" , "No se encontro ninguna coincidencia, prueba de nuevo.", "Aceptar"); }
            }

        /// <summary>
        /// Metodo que comprueba si el nombre del contacto tiene alguna coincidencia con el filtro de busqueda.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="filtro">La cadena que queremos usar como filtro. Puede estar vacia. En ese caso, devolveremos siempre true.</param>
        /// <returns> Devuelve true si se ha encontrado coincidencia, devuelve false si no la encuentra.</returns>
        public Boolean comprobarNombre(Contacto contacto, string filtro)
        {
            Boolean ok = false;
            Regex rgx = new Regex("%", RegexOptions.IgnoreCase);

            if(filtro == null)
            {
                filtro = "";
            }

            matches = rgx.Matches(filtro);

            /// Primero tenemos que controlar que en el filtro no hemos introducido mas de un %.
            if (matches.Count > 1)
            {
                inputControl(searchNameBox, "No puede indicar más de un % en una busqueda.");
            }
            else
            {
                /// Si no se ha escrito nada en el patron de busqueda, o solo se ha escrito %...
                if (filtro.Trim().Equals("") || filtro.Trim().Equals("%"))
                {
                    if (minimumAgeBox.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length == 0)
                    {
                        ok = comprobarEdad(contacto, minimumAgeBox.Text.Trim(), true);
                    }
                    else if (minimumAgeBox.Text.Trim().Length == 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, txtMaxEdad.Text.Trim(), false);
                    }
                    else if (minimumAgeBox.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, minimumAgeBox.Text.Trim(), txtMaxEdad.Text.Trim());
                    }
                    else
                    {
                        ok = true;
                    }
                }
                /// Si el ultimo caracter es %...
                else if (filtro.Substring(filtro.Length - 1).Equals("%"))
                {
                    /// Quitamos el caracter % para poder usarlo como patron de busqueda.
                    filtro = filtro.Replace("%", "");
                    rgx = new Regex(String.Format("^" + filtro + ".*"), RegexOptions.IgnoreCase);
                    matches = rgx.Matches(contacto.Nombre);
                    /// Si encuentra alguna coincidencia, devolvemos true siempre y cuando la edad tambien coincida.
                    if (matches.Count > 0 && minimumAgeBox.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length == 0)
                    {
                        ok = comprobarEdad(contacto, minimumAgeBox.Text.Trim(), true);
                    }
                    else if (matches.Count > 0 && minimumAgeBox.Text.Trim().Length == 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, txtMaxEdad.Text.Trim(), false);
                    }
                    else if (matches.Count > 0 && minimumAgeBox.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, minimumAgeBox.Text.Trim(), txtMaxEdad.Text.Trim());
                    }
                    else if (matches.Count > 0)
                    {
                        ok = true;
                    }
                }
                /// Si en el patron de busqueda no hemos puesto como ultimo caracter un %...
                else
                {
                    rgx = new Regex("^" + filtro + "$", RegexOptions.IgnoreCase);
                    matches = rgx.Matches(contacto.Nombre);
                    /// Si encuentra alguna coincidencia, devolvemos true siempre y cuando la edad tambien coincida.
                    if (matches.Count > 0 && minimumAgeBox.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length == 0)
                    {
                        ok = comprobarEdad(contacto, minimumAgeBox.Text.Trim(), true);
                    }
                    else if (matches.Count > 0 && minimumAgeBox.Text.Trim().Length == 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, txtMaxEdad.Text.Trim(), false);
                    }
                    else if (matches.Count > 0 && minimumAgeBox.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, minimumAgeBox.Text.Trim(), txtMaxEdad.Text.Trim());
                    }
                    else if (matches.Count > 0)
                    {
                        ok = true;
                    }
                }
            }

            return ok;
        }

        /// <summary>
        /// Metodo que compara la edad introducida en el formulario con la edad de un contacto.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="edad">La edad introducida al formulario para comparar.</param>
        /// <param name="modoMayor">Determina si estamos buscando mayores que la edad introducida o menos que la edad introducida</param>
        /// <returns>Devuelve true si el contacto tiene la edad correcta. Devuelve false si no cumple.</returns>
        public Boolean comprobarEdad(Contacto contacto, string edad, Boolean modoMayor)
        {
            Boolean ok = false;

            if ((Int32.Parse(contacto.Edad) < Int32.Parse(edad) && !modoMayor) || (Int32.Parse(contacto.Edad) >= Int32.Parse(edad) && modoMayor))
            {
                ok = true;
            }

            return ok;
        }

        /// <summary>
        /// Metodo que compara la edad introducida en el formulario con la edad de un contacto.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="edadMin">La edad minima introducida al formulario para comparar.</param>
        /// <param name="edadMax">La edad maxima introducida al formulario para comparar.</param>
        /// <returns>Devuelve true si el contacto tiene la edad correcta. Devuelve false si no cumple.</returns>
        public Boolean comprobarEdad(Contacto contacto, string edadMin, string edadMax)
        {
            Boolean ok = false;

            if (Int32.Parse(contacto.Edad) <= Int32.Parse(edadMax) && Int32.Parse(contacto.Edad) > Int32.Parse(edadMin))
            {
                ok = true;
            }

            return ok;
        }

        /// <summary>
        /// Muestra mensaje de error con mas de un %
        /// </summary>
        /// <param name="control"></param>
        /// <param name="mensaje"></param>
        private void inputControl(Entry control, String mensaje)
        {
            int edad;

            if (!int.TryParse(txtMaxEdad.Text.Trim(), out edad))
            {
                control.Text = "";
                //Mostrar advertencia
                DisplayAlert("Error" , mensaje, "Aceptar");
            }
        }
    }
}
