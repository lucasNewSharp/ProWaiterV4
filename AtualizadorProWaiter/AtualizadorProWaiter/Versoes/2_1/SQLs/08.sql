insert into TBEnderecosClientes
	select TBClientes.Codigo, 
		   TBClientes.Endereco, 
		   TBClientes.Bairro, 
		   TBClientes.CodCidade, 
		   TBClientes.Telefone1, 
		   TBClientes.Telefone2,
		   TBClientes.ValorEntregaPadrao,
		   TBClientes.ObservacoesPadrao
	from TBClientes