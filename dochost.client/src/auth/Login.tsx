import {SubmitHandler, useForm} from "react-hook-form";
import {LoginInput} from "../core/contracts/auth.ts";
import {useAuth} from "./AuthContext.tsx";
import {useLocation, useNavigate} from "react-router-dom";

const Login = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const {logIn} = useAuth();
    const {
        register,
        handleSubmit,
        formState: {errors}
    } = useForm<LoginInput>();

    const from = location.state?.from?.pathname || "/dashboard"

    const onSubmit: SubmitHandler<LoginInput> = (formData) => {
        logIn(formData, () => {
            navigate(from, {replace: true});
        })
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