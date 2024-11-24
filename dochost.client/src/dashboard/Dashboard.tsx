import {useQuery, useQueryClient} from "@tanstack/react-query";
import {
    API_HOST,
    downloadFile,
    fetchDocuments,
    getPreview,
    getShareableLink,
    uploadFiles
} from "../core/http/api.ts";
import React, {ChangeEvent, useMemo, useRef, useState} from "react";
import {useAuth} from "../auth/useAuth.ts";
import {DocumentInfo} from "../core/contracts/document.ts";
import LoadingIcon from "../core/components/LoadingIcon.tsx";
import UIIcons from "../core/components/UIIcons.tsx";
import FileIcons from "../core/components/FileIcons.tsx";
import {blobToBase64} from "../core/utils.ts";
import DocumentStatus from "../core/components/DocumentStatus.tsx";

function Dashboard() {
    const inputRef = useRef<HTMLInputElement>(null);
    const [shareLink, setShareLink] = useState<Record<string, string>>({});
    const [currentDocInfo, setCurrentDocInfo] = useState<DocumentInfo | null>(null);
    const [shouldPoll, setShouldPoll] = useState(true);
    const {session} = useAuth();
    const {
        isPending, error, data
    } = useQuery<DocumentInfo[]>({
        queryKey: ['documents'],
        queryFn: fetchDocuments,
        enabled: !!session?.token,
        refetchInterval: 3000
    });

    const {
        data: previewData
    } = useQuery({
        queryKey: ['preview', currentDocInfo?.id || ''],
        queryFn: async () => {
            const res = await getPreview(currentDocInfo?.id);
            return await blobToBase64(res);
        },
        enabled: !!session?.token && !!currentDocInfo && (currentDocInfo.previewStatus === "completed"),
    })

    const queryClient = useQueryClient();

    const getLink = async (fileId: string) => {
        const link = await getShareableLink(fileId);
        setShareLink({
            ...shareLink,
            [fileId]: link
        });
    }

    const handleUploadBtnClick = (e: React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        if (!inputRef || !inputRef.current) return;

        inputRef.current.click();
    }

    const handleUploadFiles = async (e: ChangeEvent<HTMLInputElement>) => {
        await uploadFiles({files: e.target.files}, async () => {
            await queryClient.invalidateQueries({queryKey: ['documents']})
        });
    }

    const composeShareLink = useMemo((): string => {
        if (!currentDocInfo) return ""
        const token = shareLink[currentDocInfo.id];
        if (!token) return ""

        return `${API_HOST}/documents/file?share=${encodeURIComponent(token)}`
    }, [currentDocInfo, shareLink]);

    if (isPending) return (
        <div className="flex justify-center items-center">
            <LoadingIcon size={96}/>
        </div>
    )

    if (error) return 'An error has occurred: ' + error.message

    if(shouldPoll && data && !data.some(info => info.previewStatus === "processing")) {
        setShouldPoll(false);
    }

    return (
        <div className="py-4 px-8 flex flex-col h-[calc(100vh-64px)]">
            <div className="flex justify-between items-center">
                <h1 className="text-3xl">Files</h1>
                <div className="flex items-center">
                    <input type="file" hidden onChange={handleUploadFiles} ref={inputRef}/>
                    <button
                        className="bg-slate-600 hover:bg-slate-700 text-white flex gap-2 border-none"
                        onClick={handleUploadBtnClick}><span>Upload</span> <UIIcons
                        type="upload"/></button>
                </div>
            </div>
            <div className="border border-slate-300 rounded-2xl my-4 flex flex-grow max-h-[calc(100vh-192px)]">
                <div className="w-2/3 p-4 overflow-y-scroll">
                    <div
                        className="border-b border-slate-300 flex gap-4 items-center px-2 pb-3 pt-4 cursor-pointer"
                    >
                        <div className="w-8 p-1">

                        </div>
                        <div className="flex items-center gap-4 flex-grow font-semibold">
                            <p className="w-1/2">File name</p>
                            <p className="w-1/4">Created on</p>
                            <p className="m-auto">Download count</p>
                        </div>
                    </div>
                    {data.map(documentInfo => {
                        return (
                            <div
                                key={documentInfo.id}
                                className="border-b border-slate-300 flex gap-4 items-center px-2 pb-3 pt-4 cursor-pointer"
                                onClick={() => {
                                    setCurrentDocInfo(documentInfo);
                                }}>
                                <div className="w-8 p-1">
                                    <FileIcons type={documentInfo.fileExt.substring(1)}/>
                                </div>
                                <div className="flex items-center gap-4 flex-grow">
                                    <p className="w-1/2">{documentInfo.fileName}</p>
                                    <p className="w-1/4">{new Date(documentInfo.createdAt).toLocaleString()}</p>
                                    <p className="m-auto">{documentInfo.downloadCount}</p>
                                    <div className="w-8">
                                        <DocumentStatus status={documentInfo.previewStatus} />
                                    </div>
                                </div>
                            </div>

                        )
                    })}
                </div>
                <div className="border-l p-4 flex flex-col gap-4 w-1/3">
                    {currentDocInfo?.uploadStatus === "completed" && (
                        <div className="flex ml-auto gap-4">
                            <input type="text" className="px-2 border border-slate-300 rounded-md"
                                   defaultValue={composeShareLink}/>
                            <button className="bg-white border border-slate-300 p-2"
                                    onClick={async () => {
                                        if (!currentDocInfo) return;
                                        await getLink(currentDocInfo.id)
                                    }}><UIIcons type="share"/>
                            </button>
                            <button className="bg-white border border-slate-300 p-2"
                                    onClick={async () => {
                                        if (!currentDocInfo) return;
                                        await downloadFile(currentDocInfo.id, currentDocInfo.fileExt, currentDocInfo.fileName)
                                    }}><UIIcons type="download"/>
                            </button>
                        </div>
                    )}
                    <div className="m-auto">
                        {previewData ? (<img
                            className="w-5/6 m-auto p-4"
                            src={previewData}
                            alt="preview"/>) : (
                            <div className=""><LoadingIcon size={96}/></div>

                        )}
                    </div>
                </div>
            </div>
            <div className="h-16"></div>
        </div>
    )
}

export default Dashboard;