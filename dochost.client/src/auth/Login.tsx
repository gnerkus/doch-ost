import {SubmitHandler, useForm} from "react-hook-form";
import {LoginInput} from "../core/contracts/auth.ts";

const Login = () => {
    const {
        register,
        handleSubmit,
        formState: { errors }
    } = useForm<LoginInput>();

    const onSubmit: SubmitHandler<LoginInput> = (formData) => {
        // todo: call login endpoint
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div>
                <label htmlFor="email">Email:</label>
                <input
                    type="email"
                    id="email"
                    {...register("email", {required: true})}
                />
                {errors.email && <span>Email is required</span>}
            </div>
            <div>
                <label htmlFor="password">Password:</label>
                <input
                    type="password"
                    id="password"
                    {...register("password", {required: true})}
                />
                {errors.password && <span>Password is required</span>}
            </div>
            <button type="submit">Submit</button>
        </form>
    );
};

export default Login;