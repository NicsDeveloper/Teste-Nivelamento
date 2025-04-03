using System;
using System.Globalization;

namespace Questao1
{
    public class ContaBancaria
    {
        public int Numero { get; init; }
        public string Titular { get; set; }

        public double Saldo { get; private set; }

        public ContaBancaria(int numero, string titular, double depositoInicial = 0)
        {
            if (depositoInicial < 0)
                throw new ArgumentException("O valor do depósito não pode ser negativo.");

            Numero = numero;
            Titular = titular;
            Saldo = depositoInicial;
        }

        public void Depositar(double valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor do depósito não pode ser negativo.");

            Saldo += valor;
        }

        public void Sacar(double valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor de saque não pode ser negativo.");

            Saldo -= valor + 3.50;
        }

        public override string ToString()
        {
            return $"Conta {Numero}, Titular: {Titular}, Saldo: $ {Saldo.ToString("F2", CultureInfo.InvariantCulture)}";
        }
    }
}
