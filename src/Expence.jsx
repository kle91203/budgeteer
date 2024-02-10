import { Select, Form, Grid, FormSelect  } from 'semantic-ui-react'

export default function Expence({amount, description, id, category}) {

    var categories = [
        {            key: 'gas',           text: 'Gas',            value: 'gas'},
        {            key: 'electricity',            text: 'Electricity',            value: 'electricity'        },
        {            key: 'groceries',            text: 'Groceries',            value: 'groceries'        },
        {            key: 'fuel',            text: 'Fuel',            value: 'fuel'        },
        {            key: 'lunch',            text: 'Lunch',            value: 'lunch'        },
        {            key: 'soda',            text: 'Soda',            value: 'soda'        },
        {            key: 'movies',            text: 'Movies',            value: 'movies'        },
        {            key: 'clothing',            text: 'Clothing',            value: 'clothing'        },
    ]

    const handleChange = (e, {id, value}) => {
      console.log(`${id} ${value}`)
    }

    return (
		<div>
      {id}
			<span>{amount}</span> <span>{description}</span> 
        <FormSelect
						fluid
            id={id + ""}
						value={category}
						options={categories}
						placeholder='Category'
            //
            // What type is x?
            onChange={(event, x) => handleChange(event, x)}
					/>
		</div>
    )
}

