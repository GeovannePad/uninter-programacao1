﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Aula3
{
    public class PraticaComExecucaoTardia
    {
        public void Exercicio1()
        {
            // Preparando cenário.
            string[] produtos = new string[] { "Notebook", "Mouse", "Teclado", "Monitor", "Fonte", "Fone" };
            var pessoas = new List<Pessoa>(5000);
            var rand = new Random();

            // Popula pessoas.
            for (int i = 0; i < 5000; i++)
            {
                pessoas.Add(new Pessoa
                {
                    Id = i,
                    Nome = $"Pessoa {i}",
                    Pedidos = new Dictionary<int, Pedido>()
                });
            }

            // Gera pedidos.
            var pedidos = new List<Pedido>(50000);

            for (int i = 0; i < 50000; i++)
            {
                var produto = produtos[rand.Next(0, 6)];
                var qtdPagamentos = rand.Next(1, 3);
                var pagador = pessoas[rand.Next(0, 5000)];

                var pedido = new Pedido()
                {
                    Id = i,
                    Data = new DateTime(rand.Next(2019, 2025), rand.Next(1, 13), rand.Next(1, 29)),
                    Produto = produto,
                    Pagamentos = new List<Pagamento>(qtdPagamentos)
                };

                pagador.Pedidos.Add(pedido.Id, pedido);
                pedidos.Add(pedido);

                for (int k = 0; k < qtdPagamentos; k++)
                {
                    var recebedor = pessoas[rand.Next(0, 5000)];
                    var idTransacao = Guid.NewGuid();

                    var pagamento = new Pagamento()
                    {
                        Pagador = pagador,
                        Recebedor = recebedor,
                        IdTransacao = idTransacao,
                        Valor = decimal.Parse($"{rand.Next(1, 10000)},{rand.Next(0, 99)}")
                    };

                    pedido.Pagamentos.Add(pagamento);
                    pedido.ValorPedido += pagamento.Valor;
                }
            }

            // Query 1
            var queryPessoasComMaiorRecebimento = pessoas.GroupBy(p => new
                                                        {
                                                            Pessoa = p,
                                                            Valor = p.Pedidos.Sum(pedido => pedido.Value.ValorPedido),
                                                            QtdPedidos = p.Pedidos.Count()
                                                        })
                                                        .OrderByDescending(x => x.Key.Valor)
                                                        .FirstOrDefault()
                                                        .Key;

            Console.WriteLine("-------------------------------");
            Console.WriteLine("Pessoa com o maior recebimento:");
            Console.WriteLine($"{queryPessoasComMaiorRecebimento.Pessoa.Nome} | {queryPessoasComMaiorRecebimento.Valor}");


            // Query 2
            var produtosMaisCaros = queryPessoasComMaiorRecebimento.Pessoa
                                                        .Pedidos
                                                        .GroupBy(x => x.Value.Produto)
                                                        .Select(x => new
                                                        {
                                                            Produto = x.Key,
                                                            Valor = x.Sum(g => g.Value.Pagamentos.Select(p => p.Valor).Sum())
                                                        })
                                                        .OrderByDescending(x => x.Valor)
                                                        .Take(5);

            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Produtos mais caros da pessoa com o maior recebimento:");
            foreach (var item in produtosMaisCaros)
            {
                Console.WriteLine($"{item.Produto} | {item.Valor}");
            }

            // Query 3
            var prodComMaisPedidos = pedidos.GroupBy(x => x.Produto)
                                            .Select(g => new
                                            {
                                                Produto = g.Key,
                                                QtdPedidos = g.Count()
                                            })
                                            .OrderByDescending(x => x.QtdPedidos);

            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Produtos com mais pedidos:");
            foreach (var item in prodComMaisPedidos)
            {
                Console.WriteLine($"{item.Produto} | {item.QtdPedidos}");
            }

        }

        public class Pessoa
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public Dictionary<int, Pedido> Pedidos { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                if (obj is not Pessoa)
                {
                    return false;
                }

                return ((Pessoa)obj).Id == Id;
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }
        public class Pedido
        {
            public int Id { get; set; }
            public DateTime Data {  get; set; }
            public string Produto { get; set; }
            public decimal ValorPedido { get; set; }
            public List<Pagamento> Pagamentos { get; set; }
        }
        public class Pagamento
        {
            public Guid IdTransacao { get; set; }
            public Pessoa Pagador { get; set; }
            public Pessoa Recebedor { get; set; }
            public decimal Valor {  get; set; }
        }
    }
}
