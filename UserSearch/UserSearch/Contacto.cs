using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LecturaFicheros
{
    public class Contacto : IEnumerable
    {
        /// <summary>
        /// Campos pertenecientes a Contacto.
        /// </summary>
        protected String nombre;
        protected int edad;
        protected String nif;

        /// <summary>
        /// Constructor del Contacto, el cual recibe nombre, edad y NIF.
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="edad"></param>
        /// <param name="NIF"></param>
        public Contacto(String nombre, int edad, String nif)
        {
            this.nombre = nombre;
            this.edad = edad;
            this.nif = nif;
        }

        public String Nombre
        {
            get
            {
                return nombre;
            }
            set
            {
                nombre = value;
            }
        }

        public string Edad
        {
            get
            {
                return edad.ToString();
            }
            set
            {
                edad = int.Parse(value);
            }
        }

        public String Nif
        {
            get
            {
                return nif;
            }
            set
            {
                nif = value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Tostring en el cual mostramos los datos del contacto.
        /// </summary>
        /// <returns>Descripción del Contacto</returns>
        override
        public string ToString()
        {
            return string.Format("{0} \n {1} \n {2}", nombre, edad, nif);
        }
    }
}
