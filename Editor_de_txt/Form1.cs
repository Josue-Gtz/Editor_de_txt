using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor_de_txt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            analizarToolStripMenuItem.Enabled = false;
            traducirToolStripMenuItem.Enabled = false;
            sintacticoToolStripMenuItem.Enabled = false;
        }
        private void abrirToolStripMenuItem_Click(object send, EventArgs e)
        {
            OpenFileDialog VentanaAbrir = new OpenFileDialog();
            VentanaAbrir.Filter = "Texto|*.c";
            if (VentanaAbrir.ShowDialog() == DialogResult.OK)
            {
                archivo = VentanaAbrir.FileName;
                using (StreamReader Leer = new StreamReader(archivo))
                {
                    richTextBox1.Text = Leer.ReadToEnd();
                }
            }
            Form1.ActiveForm.Text = "Editor de texto - " + archivo;
            analizarToolStripMenuItem.Enabled = true;
            traducirToolStripMenuItem.Enabled = true;
        }

        private void guardar()
        {
            SaveFileDialog VentanaGuardar = new SaveFileDialog();
            VentanaGuardar.Filter = "Texto|*.c";
            if (archivo != null)
            {
                using(StreamWriter Escribir = new StreamWriter(archivo))
                {
                    Escribir.Write(richTextBox1.Text);
                }
            }
            else
            {
                if(VentanaGuardar.ShowDialog() == DialogResult.OK)
                {
                    archivo = VentanaGuardar.FileName;
                    using (StreamWriter Escribir = new StreamWriter(archivo))
                    {
                        Escribir.Write(richTextBox1.Text);
                    }
                }
            }
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guardar();
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            archivo = null;
        }
        private char Tipo_caracter(int caracter)
        {
            if (caracter >= 65 && caracter <= 90 || caracter >= 97 && caracter <= 122) { return 'l'; }
            else
            {
                if(caracter >= 48 && caracter <= 57) { return 'd'; }
                else
                {
                    switch(caracter)
                    {
                        case 10:return 'n';
                        case 34:return '"';
                        case 39:return 'c';
                        case 47: return '/';
                        case 32: return 'e';


                        default:return 's';
                    };
                }
            }
        }

        private void Cadena()
        {
            do
            {
                i_caracter = Leer.Read();
                if (i_caracter == 10) Numero_linea++;
            } while(i_caracter != 34 && i_caracter != -1);
            if (i_caracter == -1) Error(-1);
        }
        private void Simbolo()
        {
            if (i_caracter == 33 ||                              // !
        (i_caracter >= 35 && i_caracter <= 38) ||        // # $ % &
        (i_caracter >= 40 && i_caracter <= 46) ||        // ( ) * + , - .   
        i_caracter == 47 ||                              // /
        (i_caracter >= 58 && i_caracter <= 62) ||        // : ; < = > ?
        i_caracter == 91 ||                              // [
        i_caracter == 93 ||                              // ]
        i_caracter == 94 ||                              // ^
        i_caracter == 123 ||                             // {
        i_caracter == 124 ||                             // |
        i_caracter == 125)                               // }
            {
                elemento = ((char)i_caracter).ToString();
                elementois = elemento + " Símbolo\n";
            }
            else
            {
                Error(i_caracter);
                elemento = "";
                elementois = "";
            }


        }

        private void Caracter()
        {
            i_caracter = Leer.Read();
            if (i_caracter != 39) Error(39);
        }
        private void Error(int i_caracter)
        {
            richTextBox2.AppendText("Error léxico " + (char)i_caracter + ", línea " + Numero_linea + "\n");
            N_error++;
        }

        private bool Comentario()
        {
            i_caracter = Leer.Read();
            switch (i_caracter)
            {
                case 47:
                    do
                    {
                        i_caracter = Leer.Read();
                    } while (i_caracter != 10);
                    return true;
                case 42:
                    do
                    {
                        do
                        {
                            i_caracter = Leer.Read();
                            if (i_caracter == 10)
                            {
                                Numero_linea++;
                            }
                        } while (i_caracter != 42 && i_caracter != -1);
                        i_caracter = Leer.Read();
                    } while (i_caracter != 47 && i_caracter != -1);
                    if (i_caracter == -1)
                    {
                        Error(i_caracter);

                    }
                    i_caracter = Leer.Read();
                    return true;
                default: return false;
            }
        }
           
        private void analizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guardar();
            N_error = 0; Numero_linea = 1;
            archivoback = archivo.Remove(archivo.Length - 1) + "back";
            Escribir = new StreamWriter(archivoback);
            Leer = new StreamReader(archivo);
            do
            {
                i_caracter = Leer.Read();
                c_caracter = (char)i_caracter;

                switch (Tipo_caracter(i_caracter))
                {
                    case 'l': elemento = "" + c_caracter;Identificador();Escribir.Write(elementois);

                        break;

                    case 'd':
                        Escribir.Write(c_caracter + "  digito\n");
                        break;

                    case 's':
                        Simbolo(); Escribir.Write(elementois); 
                        break;

                    case '"':
                        Cadena();
                        Escribir.Write("   cadena\n");
                        break;

                    case 'c':
                        Caracter();
                        Escribir.Write(c_caracter + "   caracter\n");
                        break;

                    case 'n':
                        Escribir.Write("  Salto de linea\n");
                        Numero_linea++;
                        break;

                    case 'e':
                        break;

                    case '/':
                        Comentario();
                        Escribir.Write("Comentario\n");
                        break;

                    default:
                        Error(i_caracter);
                        break;
                }
            } while (i_caracter != -1);

            Escribir.Close();
            Leer.Close();

            richTextBox2.Clear(); 
            using (StreamReader mostrar = new StreamReader(archivoback))
            {
                richTextBox2.Text = mostrar.ReadToEnd();
            }
            richTextBox2.AppendText("\nErrores: " + N_error);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            analizarToolStripMenuItem.Enabled = true;
            traducirToolStripMenuItem.Enabled = true;
            sintacticoToolStripMenuItem.Enabled = true;
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void traducirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string textoOriginal = richTextBox1.Text;
            string textoTraducido = textoOriginal;

            foreach (var palabra in P_Reservadas)
            {
                if (traducciones.ContainsKey(palabra))
                {
                    textoTraducido = System.Text.RegularExpressions.Regex.Replace(
                        textoTraducido,
                        $@"\b{palabra}\b",   
                        traducciones[palabra]);
                }
            }

            richTextBox2.Text = textoTraducido;

            
                string archivoTrad = System.IO.Path.ChangeExtension(archivo, ".trad");
                using (StreamWriter sw = new StreamWriter(archivoTrad))
                {
                    sw.Write(textoTraducido);
                }

            }
        private void Archivo_Libreria()
        {
            i_caracter = Leer.Read();
            if ((char)i_caracter == 'h')
            {
                elemento = "Libreria";      
                elementois = "Libreria\n";  
            }
            else
            {
                Error(i_caracter);
            }
        }

        // Validar si es palabra reservada
        private bool Palabra_Reservada()
        {
            if (P_Reservadas.IndexOf(elemento) >= 0) return true;
            return false;
        }

        // Identificador o palabra reservada
        private void Identificador()
        {
            do
            {
                i_caracter = Leer.Read();
                if (Tipo_caracter(i_caracter) == 'l' || Tipo_caracter(i_caracter) == 'd')
                {
                    elemento += (char)i_caracter;
                }
                else
                {
                    break;
                }
            } while (true);

            if ((char)i_caracter == '.')
            {
                Archivo_Libreria();
            }
            else
            {
                if (Palabra_Reservada())
                    elementois = elemento + "  Palabra Reservada\n";
                else
                    elementois = elemento + "  Identificador\n";
            }
        }
        private List<string> P_Reservadas  = new List<string> {
    "auto","break","case","char","const","continue","default","do","double",
    "else","enum","extern","float","for","goto","if","inline","int","long", "main",
    "register","restrict","return","short","signed","sizeof","static",
    "struct","switch","typedef","union","unsigned","void","volatile","while",
    "_Alignas","_Alignof","_Atomic","_Bool","_Complex","_Generic","_Imaginary",
    "_Noreturn","_Static_assert","_Thread_local","include","printf"
        };

        Dictionary<string,string> traducciones = new Dictionary<string, string>()
        {
    {"auto","automático"},{"break","romper"},{"case","caso"},{"char","carácter"},
        {"const","constante"},{"continue","continuar"},{"default","defecto"},{"do","hacer"},
        {"double","doble"},{"else","sino"},{"enum","enumeración"},{"extern","externo"},
        {"float","flotante"},{"for","para"},{"goto","ir_a"},{"if","si"},{"inline","en_linea"},
        {"int","entero"},{"long","largo"},{"register","registro"},{"restrict","restringido"},
        {"return","retornar"},{"short","corto"},{"signed","con_signo"},{"sizeof","tamaño_de"},
        {"static","estático"},{"struct","estructura"},{"switch","seleccionar"},
        {"typedef","definir_tipo"},{"union","unión"},{"unsigned","sin_signo"},{"void","vacío"},
        {"volatile","volátil"},{"while","mientras"},{"_Alignas","alinear_como"},
        {"_Alignof","alineación_de"},{"_Atomic","atómico"},{"_Bool","booleano"},
        {"_Complex","complejo"},{"_Generic","genérico"},{"_Imaginary","imaginario"},
        {"_Noreturn","sin_retorno"},{"_Static_assert","afirmación_estática"},
        {"_Thread_local","hilo_local"},{"include","incluir" }, {"main","principal" },

        // Funciones de stdio.h
        {"printf","imprimir"},{"scanf","leer"},{"gets","leer_linea"},{"fgets","leer_linea_segura"},
        {"puts","escribir_linea"},{"fputs","escribir_linea_segura"},{"fopen","abrir_archivo"},
        {"fclose","cerrar_archivo"},{"fread","leer_archivo"},{"fwrite","escribir_archivo"},
        {"fprintf","escribir_formato"},{"fscanf","leer_formato"},{"getc","leer_caracter"},
        {"getchar","leer_tecla"},{"putc","escribir_caracter"},{"putchar","escribir_tecla"},
        {"rewind","reiniciar_archivo"},{"fflush","vaciar_buffer"},{"remove","eliminar_archivo"},
        {"rename","renombrar_archivo"},

        // Funciones de stdlib.h
        {"malloc","reservar_memoria"},{"calloc","reservar_memoria_ceros"},
        {"realloc","redimensionar_memoria"},{"free","liberar_memoria"},{"exit","salir"},
        {"abort","abortar"},{"atexit","al_terminar"},{"system","sistema"},{"getenv","obtener_variable"},
        {"qsort","ordenar"},{"bsearch","buscar_binario"},{"abs","valor_absoluto"},
        {"labs","valor_absoluto_largo"},{"rand","aleatorio"},{"srand","semilla_aleatoria"},
        {"atoi","texto_a_entero"},{"atol","texto_a_largo"},{"atof","texto_a_decimal"},
        {"strtol","texto_a_entero_largo"},{"strtoul","texto_a_entero_sin_signo"},
        {"strtod","texto_a_decimal_doble"},

        // Funciones de string.h
        {"strlen","longitud_cadena"},{"strcpy","copiar_cadena"},{"strncpy","copiar_cadena_n"},
        {"strcat","concatenar_cadena"},{"strncat","concatenar_cadena_n"},
        {"strcmp","comparar_cadena"},{"strncmp","comparar_cadena_n"},
        {"strchr","buscar_caracter"},{"strrchr","buscar_caracter_reverso"},
        {"strstr","buscar_subcadena"},{"strtok","dividir_cadena"},{"memset","llenar_memoria"},
        {"memcpy","copiar_memoria"},{"memmove","mover_memoria"},{"memcmp","comparar_memoria"},

        // Funciones de math.h
        {"sqrt","raiz_cuadrada"},{"pow","potencia"},{"sin","seno"},{"cos","coseno"},{"tan","tangente"},
        {"asin","arcoseno"},{"acos","arcocoseno"},{"atan","arcotangente"},{"atan2","arcotangente2"},
        {"exp","exponencial"},{"log","logaritmo"},{"log10","logaritmo10"},{"ceil","techo"},
        {"floor","piso"},{"fabs","valor_absoluto_decimal"},{"fmod","residuo_decimal"},
        {"hypot","hipotenusa"},{"ldexp","escala_binaria"},{"frexp","mantisa_exponente"},
        {"modf","parte_fraccionaria"},

        // Funciones de ctype.h
        {"isalnum","es_alfanumérico"},{"isalpha","es_letra"},{"iscntrl","es_control"},
        {"isdigit","es_dígito"},{"isgraph","es_gráfico"},{"islower","es_minúscula"},
        {"isprint","es_imprimible"},{"ispunct","es_puntuación"},{"isspace","es_espacio"},
        {"isupper","es_mayúscula"},{"isxdigit","es_hexadecimal"},{"tolower","a_minúscula"},
        {"toupper","a_mayúscula"},

        // Funciones de time.h
        {"time","tiempo"},{"clock","reloj"},{"difftime","diferencia_tiempo"},
        {"mktime","tiempo_a_segundos"},{"asctime","tiempo_a_cadena"},
        {"ctime","hora_sistema"},{"gmtime","tiempo_utc"},{"localtime","tiempo_local"},
        {"strftime","formato_tiempo"}
        };






        private void ErrorS(string token)
        {
            richTextBox2.AppendText("Error sintactico " + (string)token + ", línea " + Numero_linea + "\n");
            N_error++;
        }
        private void sintacticoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            guardar();
            N_error = 0; Numero_linea = 1;
            archivoback = archivo.Remove(archivo.Length - 1) + "back";
            Escribir = new StreamWriter(archivoback);
            Leer = new StreamReader(archivo);
            do
            {
                i_caracter = Leer.Read();
                c_caracter = (char)i_caracter;

                switch (Tipo_caracter(i_caracter))
                {
                    case 'l':
                        elemento = ""+ c_caracter; Identificador(); Escribir.Write(elemento+"\n");

                        break;

                    case 'd':
                        Escribir.Write(c_caracter + "\n");
                        break;

                    case 's':
                        Simbolo(); Escribir.Write(elemento + "\n"); 
                        break;

                    case '"':
                        Cadena();
                        Escribir.Write("cadena\n");
                        break;

                    case 'c':
                        Caracter();
                        Escribir.Write(c_caracter + "\n");
                        break;

                    case 'n':
                        Escribir.Write("LF\n");
                        Numero_linea++;
                        break;

                    case 'e':
                        break;

                    case '/':
                        Comentario();
                        Escribir.Write("Comentario\n");
                        break;

                    default:
                        Error(i_caracter);
                        break;
                }

                
            } while (i_caracter != -1);

            Escribir.Close();
            Leer.Close();
            N_error = 0; Numero_linea = 1;
            Leer = new StreamReader(archivoback);
            Cabecera();
        }
        private void Cabecera()
        {
            token = Leer.ReadLine();

            switch (token)
            {
                case "#":DireProc();Numero_linea++; break;
                case "LF":Numero_linea++;Cabecera(); break;
                case "Comentario": token = Leer.ReadLine(); Numero_linea++; Cabecera(); break;
                default:ErrorS("Ya empieza el main");break;
            }
             
        }
        private void DireProc()
        {
            DireInclude();
        }

        private void DireInclude()
        {

            token =Leer.ReadLine();
            Numero_linea++;
            if (token == "include")
            {
                token = Leer.ReadLine();
                Numero_linea++;
                switch (token)
                { 
                    case "<":
                        token = Leer.ReadLine();
                        Numero_linea++;
                        if (token == "Libreria")
                        {
                            token = Leer.ReadLine();
                            Numero_linea++;
                            if (token == ">")
                            {
                                Cabecera();
                            }
                            else {
                                N_error++;
                                ErrorS("Se esperaba >");

                            }
                        }
                        else 
                        {
                            N_error++;
                            ErrorS("Se esperaba Libreria");
                        }
                        break;
                    case "cadena":Numero_linea++; Cabecera();
                        break;

                        default: N_error++; ErrorS("Se esperaba alguna directiva include "); break;


                }
            }
            else
            {
                N_error++;
                ErrorS("Se esperaba include");
            }


        }


    }
}
