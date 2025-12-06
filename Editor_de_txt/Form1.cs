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
                else Escribir.Write("identificador\n");
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
        }


        private void ErrorS(string e, string s)
        {
            richTextBox2.AppendText(" Error sintactico " + e + ", línea " + Numero_linea + " Se esperaba " + s + "\n");
            N_error++;
        }
        private void sintacticoToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

       

        private void AnalizadorSintactico()
        {
            Numero_linea = 1;
            Leer = new StreamReader(archivoback);

            SiguienteToken();

            Cabecera();
            Leer.Close();
        }

        // --- MÉTODO CENTRALIZADO PARA LEER Y CONTAR LÍNEAS ---
        private void SiguienteToken()
        {
            token = Leer.ReadLine();
            // Si tu analizador léxico guarda los saltos como "LF", esto los cuenta y los salta
            // para que el parser solo vea código útil.
            while (token == "LF")
            {
                Numero_linea++;
                token = Leer.ReadLine();
            }
        }
        // -----------------------------------------------------

        private void Cabecera()
        {
            // Nota: Ya no leemos al inicio porque AnalizadorSintactico ya leyó el primero,
            // o la llamada recursiva anterior ya dejó listo el siguiente token.

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
                case "Tipo":
                    Dec_VGlobal();
                    Cabecera();
                    break;

                case "main":
                    SiguienteToken(); // Consumimos 'main'
                    Cabecera();
                    break;

                default:
                    // Si no coincide con nada, avanzamos para evitar bucles infinitos si hay basura
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

            // Actualizamos la línea antes de buscar el siguiente símbolo (; o =)
            linea_para_error = Numero_linea;
            SiguienteToken();

            if (token == null) { Error("Se esperaba ';', '=' o '[' después del identificador"); return; }

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

            // --- CORRECCIÓN FINAL ---
            if (token != ";")
            {
                // Si el token NO es punto y coma, recuperamos la línea donde terminó la declaración.
                // Esto evita que marque el error en la línea siguiente si se saltó un LF.
                Numero_linea = linea_para_error;

                ErrorS(token, ";");
                return;
            }

            SiguienteToken();
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
    }
}
