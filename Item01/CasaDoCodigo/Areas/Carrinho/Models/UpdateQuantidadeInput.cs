﻿using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Carrinho.Model
{
    public class UpdateQuantidadeInput
    {
        public UpdateQuantidadeInput()
        {

        }

        public UpdateQuantidadeInput(string id, int quantidade)
        {
            Id = id;
            Quantidade = quantidade;
        }

        [Required]
        public string Id { get; set; }
        [Required]
        public int Quantidade { get; set; }
    }
}
