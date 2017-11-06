using LecturaFicheros;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UserSearch
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Contacto> contactos = new ObservableCollection<Contacto>();


        public MainPage()
        {
            InitializeComponent();
            readandInsertContacts();
            idListaContacto.ItemsSource = contactos;           
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



    }
}
