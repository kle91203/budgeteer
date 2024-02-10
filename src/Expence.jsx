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


    return (
		<div>
			<span>{amount}</span> <span>{description}</span> <FormSelect
						fluid
						value={category}
						options={categories}
						placeholder='Category'
					/>
		</div>
    )
}

