import {SubmitHandler, useForm} from "react-hook-form";
import {LoginInput} from "../core/contracts/auth.ts";
import {useAuth} from "./AuthContext.tsx";
import {useLocation, useNavigate} from "react-router-dom";
import {useState} from "react";

const Login = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [isLogIn, setIsLogIn] = useState(true);

    const {logIn, signUp} = useAuth();
    const {
        register,
        handleSubmit,
    } = useForm<LoginInput>();

    const from = location.state?.from?.pathname || "/dashboard"

    const onSubmit: SubmitHandler<LoginInput> = (formData) => {
        if (isLogIn) {
            logIn(formData, () => {
                navigate(from, {replace: true});
            })
        }

        signUp(formData, () => {
            setIsLogIn(true);
        })
    }

    return (
        <div>
            <div className="flex min-h-screen">
                <div className="w-1/2 bg-slate-600"></div>
                <div className="w-1/2 flex justify-center items-center">
                    <div className="flex flex-col items-center gap-4 p-4 w-96 sm:max-w-80">
                        <h1 className="text-6xl text-slate-600">Doc Host</h1>
                        <p className="text-2xl">{isLogIn ? "Sign in to your account" : "Register a new account"}</p>
                        <div className="mt-4 sm:mx-auto sm:w-full sm:max-w-sm">
                            <form
                                onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                                <div>
                                    <label htmlFor="email" className="block text-md">Email</label>
                                    <input
                                        className="mt-2 block w-full rounded-md border-0 py-1.5 sm:text-md ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-slate-600"
                                        type="email"
                                        {...register("email", {required: true})} id="email"/>
                                </div>

                                <div>
                                    <label htmlFor="password"
                                           className="block text-md">Password</label>
                                    <input
                                        className="mt-2 block w-full rounded-md border-0 py-1.5 sm:text-md ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-slate-600"
                                        type="password"
                                        {...register("password", {required: true})} id="password"/>
                                </div>
                                <div>
                                    <button
                                        className="w-full justify-center bg-slate-600 text-white rounded-md px-3 py-1.5 shadow-sm hover:bg-slate-500 focus-visible:outline transition ease-in-out"
                                        type="submit">Sign
                                        in
                                    </button>
                                </div>
                            </form>
                        </div>
                        <p className="text-gray-500">{isLogIn ? "Not yet a member?" : "Already" +
                            " have an account?"} <span></span>
                            <a onClick={() => {
                                 setIsLogIn(!isLogIn)
                            }}
                               className="font-semibold text-slate-600 hover:text-slate-700 cursor-pointer">{isLogIn ? " Create account" : " Sign in"}</a></p>
                    </div>
                </div>
            </div>
        </div>
    )
};

export default Login;