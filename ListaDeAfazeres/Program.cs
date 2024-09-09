using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Data.SqlClient;

class Program
{
    private static string connectionString;

    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        connectionString = configuration.GetConnectionString("dados");

        Console.WriteLine("MENU");
        Console.WriteLine("1. Inserir Tarefa");
        Console.WriteLine("2. Atualizar Tarefa");
        Console.WriteLine("3. Remover Tarefa");
        Console.WriteLine("4. Sair");
        Console.Write("Escolha uma opção: ");
        var opcao = Console.ReadLine();
        {
            switch (opcao)
            {
                case "1":
                    InserirTarefa();
                    break;
                case "2":
                    AtualizarTarefa();
                    break;
                case "3":
                   RemoverTarefa();
                   break;              
                case "4":
                    Console.WriteLine("Saindo...");
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    break;
            }

            Console.WriteLine("MENU");
            Console.WriteLine("1. Inserir Tarefa");
            Console.WriteLine("2. Atualizar Tarefa");
            Console.WriteLine("3. Remover Tarefa");
            Console.WriteLine("4. Sair");
            Console.Write("Escolha uma opção: ");
            opcao = Console.ReadLine();
        } while (opcao != "5") ;
    }

    static void InserirTarefa()
    {
        try
        {
            Console.Write("Digite a tarefa: ");
            var tarefa = Console.ReadLine();
            Console.WriteLine("Digite a data da tarefa (yyyy-mm-dd): ");
            var data = Console.ReadLine();
            Console.WriteLine("Digite a dificuldade: ");
            var dificuldade = Console.ReadLine();

            // Connect to MySQL database and insert data
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "INSERT INTO cad_tarefas(tarefa, data, dificuldade) VALUES (@tarefa, @data, @dificuldade)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tarefa", tarefa);
                    command.Parameters.AddWithValue("@data", data);
                    command.Parameters.AddWithValue("@dificuldade", dificuldade);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        Console.WriteLine("Tarefa inserida com sucesso.");

    }
    static void AtualizarTarefa()
    {
        try
        {
            Console.Write("Digite o ID da tarefa a ser atualizada: ");
            var idStr = Console.ReadLine();
            if (!int.TryParse(idStr, out var id) || id <= 0)
            {
                Console.WriteLine("ID inválido. Certifique-se de que é um número positivo.");
                return;
            }

            Console.Write("Digite a nova descrição da tarefa: ");
            var tarefa = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(tarefa))
            {
                Console.WriteLine("Descrição inválida. A descrição não pode estar vazia.");
                return;
            }

            Console.Write("Digite a nova data da tarefa (yyyy-mm-dd): ");
            var dataStr = Console.ReadLine();
            if (!DateTime.TryParse(dataStr, out var data))
            {
                Console.WriteLine("Data inválida. Certifique-se de que a data está no formato correto (yyyy-mm-dd).");
                return;
            }

            Console.Write("Digite a nova dificuldade da tarefa: ");
            var dificuldade = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dificuldade))
            {
                Console.WriteLine("Dificuldade inválida. A dificuldade não pode estar vazia.");
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "UPDATE cad_tarefas SET tarefa = @tarefa, data_tarefa = @data, dificuldade = @dificuldade WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tarefa", tarefa);
                    command.Parameters.AddWithValue("@data", data);
                    command.Parameters.AddWithValue("@dificuldade", dificuldade);
                    command.Parameters.AddWithValue("@id", id);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine("Tarefa atualizada com sucesso.");
                    else
                        Console.WriteLine("Nenhuma tarefa encontrada com o ID fornecido.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar tarefa: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }


    static void RemoverTarefa()
    {
        try
        {
            Console.Write("Digite o ID da tarefa a ser removida: ");
            var id = Console.ReadLine();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "DELETE FROM cad_tarefas WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao remover tarefa: {ex.Message}");
        }
        
    }
}
