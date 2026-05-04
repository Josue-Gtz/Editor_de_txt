using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//aja
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
                using (StreamWriter Escribir = new StreamWriter(archivo))
                {
                    Escribir.Write(richTextBox1.Text);
                }
            }
            else
            {
                if (VentanaGuardar.ShowDialog() == DialogResult.OK)
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
                if (caracter >= 48 && caracter <= 57) { return 'd'; }
                else
                {
                    switch (caracter)
                    {
                        case 10: return 'n';
                        case 34: return '"';
                        case 39: return 'c';
                        case 47: return '/';
                        case 32: return 'e';


                        default: return 's';
                    }
                    ;
                }
            }
        }

        private void Cadena()
        {
            do
            {
                i_caracter = Leer.Read();
                if (i_caracter == 10) Numero_linea++;

            } while (i_caracter != 34 && i_caracter != -1);
            if (i_caracter == -1) Error(-1);
        }
        private void Simbolo()
        {
            if (i_caracter == 33 ||
                i_caracter >= 35 && i_caracter <= 38 ||
                i_caracter >= 40 && i_caracter <= 45 ||
                i_caracter == 47 ||
                i_caracter >= 58 && i_caracter <= 62 ||
                i_caracter == 91 || i_caracter == 93 ||
                i_caracter == 94 || i_caracter == 123 ||
                i_caracter == 124 || i_caracter == 125)
            {
                elemento = ((char)i_caracter).ToString() + "\n";
            }
            else { Error(i_caracter); }


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



        private void Numero_Real()
        {
            do
            {
                i_caracter = Leer.Read();
            } while (Tipo_caracter(i_caracter) == 'd');

            Escribir.Write("numero_real\n");
        }
        private void Numero()
        {
            if ((char)i_caracter == '-')
            {
                i_caracter = Leer.Read();
            }

            do
            {
                i_caracter = Leer.Read();
            } while (Tipo_caracter(i_caracter) == 'd');

            if ((char)i_caracter == '.')
            {
                Numero_Real();
                return;
            }

            Escribir.Write("numero_entero\n");
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
                if (i_caracter == -1) break;
                c_caracter = (char)i_caracter;

                switch (Tipo_caracter(i_caracter))
                {
                    case 'l':
                        elemento = "" + c_caracter; Identificador(); Escribir.Write(elementois);

                        break;

                    case 'd':
                        Numero();
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
            if ((char)i_caracter == 'h') { Escribir.Write("libreria\n"); i_caracter = Leer.Read(); }
            else { Error(i_caracter); }
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
                elemento = elemento + (char)i_caracter;
                i_caracter = Leer.Read();
            } while (Tipo_caracter(i_caracter) == 'l' || Tipo_caracter(i_caracter) == 'd');

            if ((char)i_caracter == '.') { Archivo_Libreria(); }
            else
            {
                if (Palabra_Reservada()) Escribir.Write(elemento.ToLower() + "\n");
                else Escribir.Write("identificador\n"+elemento+"\n");
            }
        }
        private List<string> P_Reservadas = new List<string> {
    "auto","break","case","char","const","continue","default","do","double",
    "else","enum","extern","float","for","goto","if","inline","int","long", "main",
    "register","restrict","return","short","signed","sizeof","static",
    "struct","switch","typedef","union","unsigned","void","volatile","while",
    "_Alignas","_Alignof","_Atomic","_Bool","_Complex","_Generic","_Imaginary",
    "_Noreturn","_Static_assert","_Thread_local","include","printf"
        };

        Dictionary<string, string> traducciones = new Dictionary<string, string>()
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



        private void Error(string mensaje)
        {
            richTextBox2.AppendText($"Error sintáctico: {mensaje}, línea {Numero_linea}\n");
            N_error++;
            //Detiene todo el proceso inmediatamente.
            throw new ErrorSintacticoException("Detener análisis");
        }

        private void ErrorS(string tokenActual, string esperado)
        {
            richTextBox2.AppendText($"Error sintáctico {tokenActual}, línea {Numero_linea} Se esperaba {esperado}\n");
            N_error++;
            //Detiene todo el proceso inmediatamente.
            throw new ErrorSintacticoException("Detener análisis");
        }
        private void sintacticoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LimpiarArchivosCsv();

            guardar();

            elemento = "";
            N_error = 0;
            Numero_linea = 1;

            archivoback = archivo.Remove(archivo.Length - 1) + "back";
            Escribir = new StreamWriter(archivoback);
            Leer = new StreamReader(archivo);

            i_caracter = Leer.Read();

            do
            {
                elemento = "";

                if ((char)i_caracter == '/')
                {
                    if (Comentario())
                    {
                        Escribir.Write("Comentario\n");
                        continue;
                    }
                }

                switch (Tipo_caracter(i_caracter))
                {
                    case 'l': Identificador(); break;
                    case 'd': Numero(); break;
                    case 's': Simbolo(); Escribir.Write(elemento); i_caracter = Leer.Read(); break;
                    case '"': Cadena(); Escribir.Write("Cadena\n"); i_caracter = Leer.Read(); break;
                    case 'c': Caracter(); Escribir.Write("Caracter\n"); i_caracter = Leer.Read(); break;
                    case 'n': i_caracter = Leer.Read(); Numero_linea++; Escribir.Write("LF\n"); break;
                    case 'e': i_caracter = Leer.Read(); break;
                    default: Error(i_caracter); break;
                }
            } while (i_caracter != -1);

            Escribir.Write("Fin\n");
            richTextBox2.Clear();
            richTextBox2.AppendText("Errores: " + N_error + "\n");
            Escribir.Close();
            Leer.Close();
            AnalizadorSintactico();


        }

        private void tipito()
        {
            string[] lineas = File.ReadAllLines(archivoback);

            using (StreamWriter writer = new StreamWriter(archivoback))
            {
                foreach (string palabra in lineas)
                {
                    // Si la palabra está en nuestra lista de tipos
                    if (P_Res_Tipo.Contains(palabra.Trim()))
                    {
                        writer.WriteLine("tipo"); // Escribimos la etiqueta arriba
                    }

                    // Siempre escribimos la palabra original (esté o no en la lista)
                    writer.WriteLine(palabra);
                }
            }
        }


        private void AnalizadorSintactico()
        {

            tipito();
            Numero_linea = 1;
            Leer = new StreamReader(archivoback);

            SiguienteToken();

            try
            {
                Cabecera();

                // Si llega aquí, es que no hubo errores
                richTextBox2.AppendText("Análisis completado con éxito.\n");
            }
            catch (ErrorSintacticoException ex)
            {

                richTextBox2.AppendText("El análisis se detuvo debido al error anterior.\n");
            }

            Leer.Close();
        }

        private void SiguienteToken()
        {
            token = Leer.ReadLine();

            while (token == "LF")
            {
                Numero_linea++;
                token = Leer.ReadLine();
            }
        }

        private void Cabecera()
        {
            origen = "Global";
            if (token == null || token == "Fin") return;

            switch (token)
            {
                case "#":
                    SiguienteToken(); // Avanzamos
                    if (token == null) { Error("Directiva incompleta después de '#'"); return; }
                    Directiva_proc();
                    Cabecera();
                    break;

                // Tipos de datos
                case "int":
                case "float":
                case "double":
                case "char":
                case "tipo":
                    SiguienteToken();
                    Dec_VGlobal();
                    Cabecera();
                    break;

                case "main":
                    origen = "main";
                    Funcion_Main();
                    break;




                default:
                    SiguienteToken();
                    Cabecera();
                    break;
            }
        }

        private void Directiva_proc()
        {
            // El while de LF ya no es necesario aquí porque SiguienteToken lo maneja,
            // pero validamos si llegó null.
            if (token == null)
            {
                Error("Directiva incompleta después de '#'");
                return;
            }

            switch (token)
            {
                case "include":
                    Directiva_include();
                    break;

                case "define":
                    SiguienteToken(); // Leemos lo que sigue al define
                    if (token == null)
                    {
                        Error("Directiva 'define' incompleta.");
                    }
                    break;

                default:
                    Error($"Se esperaba 'include' o 'define' después de '#', pero se encontró '{token}'");
                    break;
            }
        }

        private void Directiva_include()
        {
            SiguienteToken(); // Leemos el siguiente token después de 'include'

            switch (token)
            {
                case "<":
                    SiguienteToken();
                    if (token == "libreria")
                    {
                        SiguienteToken();
                        if (token == ">")
                        {
                            SiguienteToken(); // Consumir el '>'
                        }
                        else
                        {
                            N_error++;
                            Error("Se esperaba >");
                        }
                    }
                    else
                    {
                        N_error++;
                        Error("Se esperaba Libreria");
                    }
                    break;

                case "Cadena":
                    SiguienteToken(); // Consumir la cadena
                    break;

                default:
                    N_error++;
                    Error("Se esperaba alguna directiva include");
                    break;
            }
        }

        private void Dec_VGlobal()
        {
            tipoVar = token;
            // 1. Variable para recordar la línea donde termina la instrucción
            int linea_para_error = Numero_linea;

            // Leer identificador
            SiguienteToken();
            if (token == null) { Error("Se esperaba identificador después del tipo de dato"); return; }

            if (token != "identificador")
            {
                ErrorS(token, "identificador");
                return;
            }
            SiguienteToken();
            nombreVar = token;
            // Actualizamos la línea antes de buscar el siguiente símbolo (; o =)
            linea_para_error = Numero_linea;
            SiguienteToken();

            if (token == null) { Error("Se esperaba ';', '=' , '[' o '(' después del identificador"); return; }
            origenF = token;
            
            if (token == "(")
            {

                SiguienteToken();
                Funcion();
                return;

            }

            // Manejar arreglos
            while (token == "[")
            {
                SiguienteToken();
                if (token == null) { Error("Se esperaba tamaño de arreglo"); return; }

                if (token != "numero_entero" && token != "identificador")
                {
                    ErrorS(token, "número entero o identificador para tamaño del arreglo");
                    return;
                }

                SiguienteToken();
                if (token != "]")
                {
                    ErrorS(token, "]");
                    return;
                }

                // Actualizamos línea antes de avanzar
                linea_para_error = Numero_linea;
                SiguienteToken();
                if (token == null) { Error("Se esperaba ';' o '=' después del arreglo"); return; }
            }




            // Inicialización opcional
            if (token == "=")
            {
                SiguienteToken();
                if (token == null) { Error("Se esperaba valor después de '='"); return; }

                if (token == "{")
                {
                    BloqueInicializacion();

                    // Verificación específica para bloques {}
                    if (token != ";")
                    {
                        // Si falta punto y coma aquí, usamos la línea actual
                        // (BloqueInicializacion ya maneja sus avances)
                        ErrorS(token, ";");
                        return;
                    }
                    SiguienteToken();
                    return;
                }

                if (token == "-") SiguienteToken();
                if (token == null) { Error("Se esperaba valor después de '-'"); return; }

                if (token != "numero_entero" && token != "numero_real" && token != "Cadena" && token != "caracter")
                {
                    ErrorS(token, "valor válido para inicialización");
                    return;
                }

                // Actualizamos línea por si era un entero/char/cadena y aquí termina
                linea_para_error = Numero_linea;
                SiguienteToken();

                if (token == ".")
                {
                    SiguienteToken();
                    if (token != "numero_entero")
                    {
                        ErrorS(token, "número después del punto decimal");
                        return;
                    }

                    // Actualizamos línea porque estamos al final del float
                    linea_para_error = Numero_linea;
                    SiguienteToken();
                }
            }

            if (token != ";")
            {
                //&& token != "("
                Numero_linea = linea_para_error;

                ErrorS(token, "; o ( ");
                return;
            }


            SiguienteToken();
            RegistrarVariableEnCsv(nombreVar,tipoVar,origen);
        }

        private void BloqueInicializacion()
        {
            if (token != "{")
            {
                ErrorS(token, "{");
                return;
            }

            SiguienteToken();

            while (token != "}")
            {
                if (token == "{")
                {
                    BloqueInicializacion();
                }
                else if (token == "numero_entero" || token == "numero_real" || token == "identificador" || token == "Cadena" || token == "caracter")
                {
                    SiguienteToken();
                }
                else
                {
                    ErrorS(token, "valor válido o sub-arreglo dentro de inicialización");
                    return;
                }

                if (token == ",")
                {
                    SiguienteToken();
                }
                else if (token == "}")
                {
                    break;
                }
                else
                {
                    ErrorS(token, "',' o '}'");
                    return;
                }
            }

            SiguienteToken(); // Consumir la llave de cierre '}'
        }


        private void Funcion_Main()
        {
            SiguienteToken(); // main

            if (token != "(") { ErrorS(token, "("); return; }
            SiguienteToken();

            if (token != ")") { ErrorS(token, ")"); return; }
            SiguienteToken();


            BloqueDeSentencias();
        }





        // Una clase simple para identificar cuando paramos por error
        public class ErrorSintacticoException : Exception
        {
            public ErrorSintacticoException(string message) : base(message) { }
        }



        //Analizador de expresiones condicionales y aritméticas

        // Nivel 1: Entrada principal (Maneja || y | |)
        private void Condicion()
        {
            TerminoAND();

            // Detectamos "||" junto o separado "|" + "|"
            while (token == "||" || token == "|")
            {
                if (token == "|")
                {
                    SiguienteToken();
                    if (token != "|") { ErrorS(token, "| (para completar el operador OR)"); return; }
                }
                SiguienteToken(); // Consumimos el segundo '|' o el token "||"
                TerminoAND();
            }
        }

        // Nivel 2: Maneja && y & &
        private void TerminoAND()
        {
            ExpresionIgualdad();

            // Detectamos "&&" junto o separado "&" + "&"
            while (token == "&&" || token == "&")
            {
                if (token == "&")
                {
                    SiguienteToken();
                    if (token != "&") { ErrorS(token, "& (para completar el operador AND)"); return; }
                }
                SiguienteToken(); // Consumimos el segundo '&' o el token "&&"
                ExpresionIgualdad();
            }
        }

        // Nivel 3: Maneja == y != (Con corrección para tokens separados)
        private void ExpresionIgualdad()
        {
            ExpresionRelacional();

            // Verificamos si es un operador de igualdad
            bool esOperador = false;

            // Caso especial: != o ! =
            if (token == "!" || token == "!=")
            {
                if (token == "!")
                {
                    SiguienteToken();
                    if (token == "=") { esOperador = true; SiguienteToken(); }
                    else
                    {

                    }
                }
                else
                { // token es "!="
                    esOperador = true;
                    SiguienteToken();
                }
            }
            // Caso especial: == o = =
            else if (token == "==" || token == "=")
            {
                if (token == "=")
                {
                    SiguienteToken();
                    if (token == "=") { esOperador = true; SiguienteToken(); }

                }
                else
                {
                    esOperador = true;
                    SiguienteToken();
                }
            }

            if (esOperador)
            {
                ExpresionRelacional();
            }
        }

        // Nivel 4: Maneja <, >, <=, >= (Con corrección para tokens separados)
        private void ExpresionRelacional()
        {
            ExpresionAritmetica(); // Lado izquierdo

            string op = token;
            bool esOperador = false;

            // Detectar <, >, <=, >=, o separados < =, > =
            if (op == "<" || op == ">" || op == "<=" || op == ">=")
            {
                esOperador = true;
                SiguienteToken();

                // Si el token era solo < o >, miramos si sigue un =
                if ((op == "<" || op == ">") && token == "=")
                {
                    SiguienteToken(); // Consumimos el '=' extra (forma <= o >=)
                }
            }

            if (esOperador)
            {
                ExpresionAritmetica(); // Lado derecho
            }
        }

        // Nivel 5: Expresiones matemáticas simples (Suma/Resta) - Tu "Operando"
        private void ExpresionAritmetica()
        {
            Termino(); // Multiplicación/División

            while (token == "+" || token == "-")
            {
                SiguienteToken();
                Termino();
            }
        }

        // Nivel 6: Multiplicación y División
        private void Termino()
        {
            Factor();
            while (token == "*" || token == "/")
            {
                SiguienteToken();
                Factor();
            }
        }

        // Nivel 7: El dato base (Números, IDs, Paréntesis, NOT, Menos Unario)
        private void Factor()
        {
            // 1. Manejo de operadores unarios (!, -, +)
            if (token == "!" || token == "-" || token == "+")
            {
                SiguienteToken();
                Factor(); // Recursividad: volvemos a llamar a Factor para leer el número que sigue
                return;
            }

            // 2. Paréntesis 
            if (token == "(")
            {
                SiguienteToken();
                ExpresionNueva(); // Volvemos arriba para permitir (a < b || c > d)
                if (token != ")")
                {
                    ErrorS(token, ")");
                }
                SiguienteToken();
            }
            // 3. Identificadores y Números
            else if (token == "identificador" || token == "numero_entero" || token == "numero_real")
            {

                SiguienteToken();
            }
            else
            {
                // Si llegamos aquí y no es nada de lo anterior, es un error
                ErrorS(token, "identificador, número, '(' o signo '-'");
            }
        }






        //GESTOR DE BLOQUES Y SENTENCIAS
        private void BloqueDeSentencias()
        {
            if (token != "{")
            {
                ErrorS(token, "{ (Se requiere abrir bloque con llave)");
                return;
            }

            SiguienteToken();
            // Ciclo principal del bloque: lee hasta encontrar } o el fin del archivo
            while (token != "}" && token != "Fin" && token != null)
            {
                switch (token)
                {
                    // Variables
                    case "int":
                    case "float":
                    case "double":
                    case "char":
                    case "tipo":
                        SiguienteToken();
                        Dec_VGlobal();
                        break;

                    // Estructuras de Control 
                    case "if": EstructuraIf(); break;
                    case "while": EstructuraWhile(); break;
                    case "do": EstructuraDoWhile(); break;
                    case "for": EstructuraFor(); break;
                    case "switch": EstructuraSwitch(); break;

                    // Rupturas
                    case "break":
                    case "continue":
                    case "return":
                        SiguienteToken();
                        if (token == ";") SiguienteToken();
                        else ErrorS(token, "; después de break/return");
                        break;

                    // Bloques anidados (Ámbitos hijos)
                    case "{":
                        BloqueDeSentencias();
                        break;

                    case ";": SiguienteToken(); break;

                    // Asignaciones o llamadas
                    case "identificador":
                        SiguienteToken();
                        Sentencia();
                        break;

                    default:
                        ErrorS(token, "declaración o sentencia válida dentro del bloque o cierre '}'");
                        SiguienteToken();
                        break;
                }
            }

            if (token == "}")
            {
                SiguienteToken();
            }
            else
            {
                ErrorS(token, "}");
            }
        }

        //                         SENTENCIA SIMPLE

        private void Sentencia()
        {
            SiguienteToken();

            // Caso 1: Asignación ( a = )
            if (token == "=")
            {
                SiguienteToken();


                ExpresionNueva();

                if (token == ";") SiguienteToken();
                else ErrorS(token, "; al final de la asignación");
            }
            // Caso 2: Llamada a función o expresión sola 
            else if (token == ";")
            {
                SiguienteToken();
            }
            // Caso 3: Operadores unarios comunes como ++ o -- (si tu lexer los soporta)
            else if (token == "+" || token == "-")
            {
                // Consumir el segundo + o - si existe y luego el ;
                SiguienteToken();
                if (token == ";") SiguienteToken();
            }
            else
            {
                ErrorS(token, "= o ;");
            }
        }

        private void SentenciaOBloque()
        {
            if (token == "{")
            {
                BloqueDeSentencias();
            }
            else
            {
                // Si no es bloque, DEBE ser una sentencia simple (asignación/llamada)
                if (token == "identificador")
                {
                    Sentencia();
                }
                else if (token == ";")
                {
                    SiguienteToken();
                }
                else
                {
                    ErrorS(token, "sentencia o '{'");
                }
            }
        }

        //ESTRUCTURAS DE CONTROL

        // if else
        private void EstructuraIf()
        {
            SiguienteToken(); // if
            if (token != "(") { ErrorS(token, "("); return; }
            SiguienteToken();
            ExpresionNueva();
            if (token != ")") { ErrorS(token, ")"); return; }
            SiguienteToken();

            BloqueDeSentencias();

            if (token == "else")
            {
                SiguienteToken();
                BloqueDeSentencias();
            }
        }

        // while
        private void EstructuraWhile()
        {
            SiguienteToken(); // while
            if (token != "(") { ErrorS(token, "("); return; }
            SiguienteToken();
            ExpresionNueva();
            if (token != ")") { ErrorS(token, ")"); return; }
            SiguienteToken();

            BloqueDeSentencias();
        }

        // dowhile
        private void EstructuraDoWhile()
        {
            SiguienteToken();

            SentenciaOBloque();

            if (token != "while") { ErrorS(token, "while"); return; }
            SiguienteToken();

            if (token != "(") { ErrorS(token, "("); return; }
            SiguienteToken();

            ExpresionNueva();

            if (token != ")") { ErrorS(token, ")"); return; }
            SiguienteToken();

            if (token != ";") { ErrorS(token, ";"); return; }
            SiguienteToken();
        }

        // FOR
        private void EstructuraFor()
        {
            SiguienteToken();
            if (token != "(") { ErrorS(token, "("); return; }
            SiguienteToken();

            // 1. Inicialización (Opcional)
            if (token != ";")
            {
                if (token == "int" || token == "float") Dec_VGlobal(); // Declaración local en for
                else if (token == "identificador") Sentencia();
                else { /* Manejar error o vacío */ }
            }
            else SiguienteToken(); // Si estaba vacío, consumimos ;

            // 2. Condición 
            if (token != ";")
            {
                ExpresionNueva();
            }
            if (token != ";") { ErrorS(token, "; separador en for"); return; }
            SiguienteToken();

            // 3. Progresión 
            if (token != ")")
            {
                if (token == "identificador")
                {
                    SiguienteToken();
                    if (token == "=")
                    {
                        SiguienteToken();
                        ExpresionNueva(); // Evaluamos la expresión
                    }
                }
            }

            if (token != ")") { ErrorS(token, ")"); return; }
            SiguienteToken();

            BloqueDeSentencias();
        }

        // switch
        private void EstructuraSwitch()
        {
            SiguienteToken();
            if (token != "(") { ErrorS(token, "("); return; }
            SiguienteToken();

            // Expresión a evaluar 

            if (token != ")") { ErrorS(token, ")"); return; }
            SiguienteToken();

            if (token != "{") { ErrorS(token, "{"); return; }
            SiguienteToken();

            // Cuerpo del Switch
            while (token != "}" && token != "Fin")
            {
                if (token == "case")
                {
                    SiguienteToken();
                    // Se espera una constante (número o char)
                    if (token == "numero_entero" || token == "caracter")
                    {
                        SiguienteToken();
                    }
                    else ErrorS(token, "constante o caracter para case");

                    if (token != ":") ErrorS(token, ":");
                    else SiguienteToken();

                    // Aquí pueden venir sentencias hasta el siguiente case/default/}
                    // Para simplificar: Leemos sentencias sueltas hasta ver 'case', 'default' o '}'
                    while (token != "case" && token != "default" && token != "}" && token != "Fin")
                    {
                        // Usamos un switch interno o llamamos lógica similar a Bloque

                        // Solución rápida: Permitir una sola instrucción o bloque, o break.
                        if (token == "break") { SiguienteToken(); if (token == ";") SiguienteToken(); }
                        else if (token == "{") BloqueDeSentencias();
                        else if (token == "identificador") Sentencia();
                        else break; // Salir si no reconocemos nada 
                    }
                }
                else if (token == "default")
                {
                    SiguienteToken();
                    if (token != ":") ErrorS(token, ":");
                    else SiguienteToken();

                    // Mismo tratamiento que case
                    while (token != "case" && token != "}" && token != "Fin")
                    {
                        if (token == "break") { SiguienteToken(); if (token == ";") SiguienteToken(); }
                        else if (token == "{") BloqueDeSentencias();
                        else if (token == "identificador") Sentencia();
                        else break;
                    }
                }
                else
                {
                    if (token != "}") SiguienteToken();
                }
            }
            if (token == "}") SiguienteToken();
        }




        private void Funcion()
        {

            origenAUX = origen;
            origen = origenF;
            while (token != "{")
            {

                if (token == "tipo")
                {
                    SiguienteToken();
                    tipoT = token;

                    SiguienteToken();
                    if (token == "identificador")
                    {
                        SiguienteToken();
                        nombreT = token;

                        SiguienteToken();
                        preDatos = tipoT + " " + nombreT+ ",";
                        TipoDeDatos.Add(preDatos);
                        tipoDato = String.Join(", ", TipoDeDatos);
                        if (token != "," && token != ")")
                        {
                            ErrorS(token, ", o )"); return;
                        }
                        else if (token == ")")
                        {
                            n_para += 1;  SiguienteToken();
                            
                            if (token == ";") { return; }
                            else { origen = origenAUX; RegistrarFuncionEnCsv(nombreVar, tipoVar, n_para, tipoDato); BloqueDeSentencias();  return; }
                        }
                        else { SiguienteToken(); }

                    }
                    else
                    {
                        Error("Se esperaba identificadora "); return;
                    }

                }

                else if (token == ")")
                {
                    SiguienteToken();

                    if (token == "{")
                    { BloqueDeSentencias(); return; }
                    else if (token == ";") { return; }
                    else { ErrorS(token, "; o {"); return; }

                }
                else
                {
                    ErrorS(token, " tipo de variable "); return;
                }
            }

            if (token == "{")
            { BloqueDeSentencias(); return; }
        }

        public void RegistrarVariableEnCsv(string nombre, string tipo, string origen = "")
        {
            string archivoSalida = "tablaV.csv";

            if (!File.Exists(archivoSalida))
            {
                File.WriteAllText(archivoSalida, "Origen,Nombre,Tipo\n", Encoding.UTF8);
            }

            
            string nuevaFila = $"{origen},{nombre},{tipo}\n";

            File.AppendAllText(archivoSalida, nuevaFila, Encoding.UTF8);
        }


        public void RegistrarFuncionEnCsv(string nombre, string tipo, int numero_para, string Tipos_datos)
        {
            string archivoSalida = "tablaF.csv";

            if (!File.Exists(archivoSalida))
            {
                File.WriteAllText(archivoSalida, "Nombre,Tipo,Numero de parametros,Tipos de datos\n", Encoding.UTF8);
            }

           
            string nuevaFila = $"{nombre},{tipo},{numero_para},{Tipos_datos}\n";

            File.AppendAllText(archivoSalida, nuevaFila, Encoding.UTF8);
        }




        /// <summary>
      
        /// </summary>
        

        // 2. Función para los OPERANDOS
        private void Operandos()
        {
            switch (token)
            {
                case "identificador":
                    // Recordando tu lexer: hacemos el doble salto
                    SiguienteToken(); // Consume la palabra "identificador"
                    SiguienteToken(); // Consume el nombre de la variable

                    // Verificamos si es una "Invocación a función"
                    if (token == "(")
                    {
                        SiguienteToken(); // Consumimos el '('

                        // Leemos los argumentos separados por comas
                        while (token != ")" && token != "Fin" && token != null)
                        {
                            ExpresionNueva();
                            if (token == ",") SiguienteToken();
                        }

                        if (token == ")") SiguienteToken();
                        else ErrorS(token, ") para cerrar la invocación a la función");
                    }
                    break;

                case "numero_entero":
                case "numero_real":
                case "caracter":
                case "true":
                case "false":
                    SiguienteToken(); // Todos estos solo ocupan avanzar una vez
                    break;

                default:
                    ErrorS(token, "identificador, número, caracter o booleano");
                    break;
            }
        }



        // 1. Función principal de EXPRESIÓN
        private void ExpresionNueva()
        {
            // 1. Siempre leemos el primer lado de la expresión (ej. el '12' o un paréntesis)
            ElementoExpresion();

            // 2. Mientras haya operadores binarios, iteramos para leer el lado derecho
            while (token == "+" || token == "-" || token == "*" || token == "/" || token == "%" ||
                   token == "<" || token == ">" || token == "=" || token == "!" || token == "&" || token == "|")
            {
                OperadorBinario();   // Consumimos el operador (ej. '/')
                ElementoExpresion(); // Leemos lo que le sigue (ej. el '(' y todo su contenido)
            }
        }

        // 2. Función auxiliar para leer los componentes de la expresión
        // 2. Función auxiliar para leer los componentes de la expresión
        private void ElementoExpresion()
        {
            // Caso A: Es una sub-expresión entre paréntesis
            if (token == "(")
            {
                SiguienteToken(); // Consumimos el '('

                ExpresionNueva(); // ¡RECURSIVIDAD! Evaluamos todo lo que está adentro

                if (token == ")")
                {
                    SiguienteToken(); // Consumimos el ')' y terminamos este elemento
                }
                else
                {
                    ErrorS(token, ")");
                }
            }
            // Caso B: Es un valor normal con sus posibles unarios (ej. -5 o !true)
            else if (token == "+" || token == "-" || token == "!" || token == "~" ||
                     token == "identificador" || token == "numero_entero" ||
                     token == "numero_real" || token == "caracter" ||
                     token == "true" || token == "false")
            {
                OpUnarios(); // Leemos el unario ANTES del operando (ej. el '-' de -5)
                Operandos(); // Leemos el operando en sí (ej. el 5 o la variable x)

                // ¡ELIMINAMOS la segunda llamada a OpUnarios() que estaba aquí!
            }
            // Caso C: Si no es paréntesis ni valor, la expresión está mal formada
            else
            {
                ErrorS(token, "expresión válida");
            }
        }




        private void OpUnarios()
        {
            while (token == "+" || token == "-" || token == "!" || token == "~")
            {
                switch (token)
                {
                    case "+":
                        SiguienteToken(); // Consume el primer '+'
                        if (token == "+") { SiguienteToken(); } // Si es '++', lo consume
                                                                // No necesitamos 'else'. Si es solo '+', ya lo consumió arriba.
                        break;

                    case "-":
                        SiguienteToken(); // Consume el primer '-'
                        if (token == "-") { SiguienteToken(); } // Si es '--', lo consume
                                                                // Igual aquí, borramos el else.
                        break;

                    case "!":
                    case "~":
                        SiguienteToken(); // Solo avanzamos el token
                        break;
                }
            }
        }

        private void OperadorBinario()
        {
            switch (token)
            {
                // Operadores de un solo símbolo
                case "*":
                case "+":
                case "-":
                case "/":
                case "%":
                    SiguienteToken();
                    break; // Como es void, usamos break en lugar de return true

                // Operadores que pueden ser simples (<, >) o dobles (<=, >=)
                case "<":
                case ">":
                    SiguienteToken();
                    if (token == "=") SiguienteToken();
                    break;

                // Operadores estrictamente dobles (==)
                case "=":
                    SiguienteToken();
                    if (token == "=")
                    {
                        SiguienteToken();
                    }
                    else
                    {
                        ErrorS(token, "= (para formar el operador ==)");
                    }
                    break;

                // Operadores estrictamente dobles (!=)
                case "!":
                    SiguienteToken();
                    if (token == "=")
                    {
                        SiguienteToken();
                    }
                    else
                    {
                        ErrorS(token, "= (para formar el operador !=)");
                    }
                    break;

                // Operadores lógicos estrictamente dobles (&&)
                case "&":
                    SiguienteToken();
                    if (token == "&")
                    {
                        SiguienteToken();
                    }
                    else
                    {
                        ErrorS(token, "& (para formar el operador &&)");
                    }
                    break;

                // Operadores lógicos estrictamente dobles (||)
                case "|":
                    SiguienteToken();
                    if (token == "|")
                    {
                        SiguienteToken();
                    }
                    else
                    {
                        ErrorS(token, "| (para formar el operador ||)");
                    }
                    break;

                // Si llegamos aquí por error
                default:
                    ErrorS(token, "operador binario");
                    break;
            }
        }


        private void LimpiarArchivosCsv()
        {
            // 1. Limpiamos el archivo de Variables
            string archivoVariables = "tablaV.csv";
            if (File.Exists(archivoVariables))
            {
                File.Delete(archivoVariables);
            }

            // 2. Limpiamos el archivo de Funciones
            string archivoFunciones = "tablaF.csv";
            if (File.Exists(archivoFunciones))
            {
                File.Delete(archivoFunciones);
            }
        }


    }

}
