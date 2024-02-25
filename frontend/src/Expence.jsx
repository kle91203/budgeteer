import Select from 'react-select';

export default function Expence({amount, description, id, category}) {
    var categories = [
      {label: 'Gas', value: 'gas'},
      {label: 'Electricity', value: 'electricity'},
      {label: 'Groceries', value: 'groceries'},
      {label: 'Fuel', value: 'fuel'},
      {label: 'Lunch', value: 'lunch'},
      {label: 'Soda', value: 'soda'},
      {label: 'Movies', value: 'movies'},
      {label: 'Clothing', value: 'clothing'},
    ]

    const handleChange = (e, id) => {
      console.log(`${id} ${JSON.stringify(e)}`)
    }

    return (
		<div>
      {id}
			<span>{amount}</span> <span>{description}</span> 
        <Select options={categories} placeholder="Select an option" onChange={event => handleChange(event, id)} />;
		</div>
    )
}

